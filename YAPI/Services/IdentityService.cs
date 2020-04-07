using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YAPI.Data;
using YAPI.Domain;
using YAPI.Options;

namespace YAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly JwtSettings options;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly DataContext dataContext;

        public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JwtSettings> options, TokenValidationParameters tokenValidationParameters, DataContext dataContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.options = options.Value;
            this.tokenValidationParameters = tokenValidationParameters;
            this.dataContext = dataContext;
        }

        public async Task<AuthenticationResult> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthenticationResult { ErrorMessage = new[] { "user doesnt exist" } };

            var userHasValidPassword = await userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
                return new AuthenticationResult { ErrorMessage = new[] { "password is invalid" } };

            return await GetAuthenticationResultAsync(user);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
                return new AuthenticationResult { ErrorMessage = new[] { "Invalid token" } };

            //token expire time
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationResult { ErrorMessage = new[] { "this token hasnt expired yet" } };

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
                return new AuthenticationResult { ErrorMessage = new[] { "this refresh token doesnt exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationResult { ErrorMessage = new[] { "this refresh token has expired" } };
            if (storedRefreshToken.InValidated)
                return new AuthenticationResult { ErrorMessage = new[] { "this refresh token has been invalidate" } };
            if (storedRefreshToken.Used)
                return new AuthenticationResult { ErrorMessage = new[] { "this refresh token has been used" } };
            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResult { ErrorMessage = new[] { "this refresh doesnt match this JWT" } };

            storedRefreshToken.Used = true;
            dataContext.RefreshTokens.Update(storedRefreshToken);
            await dataContext.SaveChangesAsync();

            var user = await userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == "Id").Value);
            return await GetAuthenticationResultAsync(user);

        }
        /// <summary>
        /// validates given token is valid
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var _tokenValidationParameters = tokenValidationParameters.Clone();
                _tokenValidationParameters.ValidateLifetime = false;//to validate token even it s expired
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }

        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<AuthenticationResult> RegisterAsync(string email, string password)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return new AuthenticationResult { ErrorMessage = new[] { "user with this email exist" } };

            var newUser = new AppUser
            {
                Email = email,
                UserName = email
            };

            var createdUser = await userManager.CreateAsync(newUser, password);//gonaa hash this pass mofo
            if (!createdUser.Succeeded)
                return new AuthenticationResult { ErrorMessage = createdUser.Errors.Select(x => x.Description) };

            //token
            return await GetAuthenticationResultAsync(newUser);
        }

        private async Task<AuthenticationResult> GetAuthenticationResultAsync(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(options.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Email),
                    new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email),
                    new Claim(type: "Id", value: user.Id),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                }),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.Add(options.TokenLifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = user.Id,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6)
            };

            await dataContext.RefreshTokens.AddAsync(refreshToken);
            await dataContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                Success = true,
                ErrorMessage = null

            };
        }
    }
}
