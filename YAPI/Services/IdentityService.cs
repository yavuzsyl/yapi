using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Responses;
using YAPI.Data;
using YAPI.Domain;
using YAPI.Options;

namespace YAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly JwtSettings options;
        private readonly TokenValidationParameters tokenValidationParameters;
        private readonly DataContext dataContext;

        public IdentityService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, IOptions<JwtSettings> options, TokenValidationParameters tokenValidationParameters, DataContext dataContext)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.options = options.Value;
            this.tokenValidationParameters = tokenValidationParameters;
            this.dataContext = dataContext;
        }
        //public static int counter = 0; used to add roles to user 
        public async Task<AuthenticationResponse> LoginAsync(string email, string password)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return new AuthenticationResponse { ErrorMessage = new[] { "user doesnt exist" } };

            var userHasValidPassword = await userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPassword)
                return new AuthenticationResponse { ErrorMessage = new[] { "password is invalid" } };

            //if (counter == 0)
            //{
            //await userManager.AddToRoleAsync(user, "Admin");
            //    counter++;
            //}
            //else
            //await userManager.AddToRoleAsync(user, "Poster");


            return await GetAuthenticationResultAsync(user);
        }

        public async Task<AuthenticationResponse> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);
            if (validatedToken == null)
                return new AuthenticationResponse { ErrorMessage = new[] { "Invalid token" } };

            //token expire time
            var expiryDateUnix = long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
                return new AuthenticationResponse { ErrorMessage = new[] { "this token hasnt expired yet" } };

            var jti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = await dataContext.RefreshTokens.SingleOrDefaultAsync(x => x.Token == refreshToken);
            if (storedRefreshToken == null)
                return new AuthenticationResponse { ErrorMessage = new[] { "this refresh token doesnt exist" } };

            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                return new AuthenticationResponse { ErrorMessage = new[] { "this refresh token has expired" } };
            if (storedRefreshToken.InValidated)
                return new AuthenticationResponse { ErrorMessage = new[] { "this refresh token has been invalidate" } };
            if (storedRefreshToken.Used)
                return new AuthenticationResponse { ErrorMessage = new[] { "this refresh token has been used" } };
            if (storedRefreshToken.JwtId != jti)
                return new AuthenticationResponse { ErrorMessage = new[] { "this refresh doesnt match this JWT" } };

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

        public async Task<AuthenticationResponse> RegisterAsync(string email, string password)
        {
            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser != null)
                return new AuthenticationResponse { ErrorMessage = new[] { "user with this email exist" } };

            var newUserId = Guid.NewGuid();
            var newUser = new AppUser
            {
                Id = newUserId.ToString(),
                Email = email,
                UserName = email
            };

            var createdUser = await userManager.CreateAsync(newUser, password);//gonaa hash this pass mofo
            if (!createdUser.Succeeded)
                return new AuthenticationResponse { ErrorMessage = createdUser.Errors.Select(x => x.Description) };

            //added claim for policy authorization
            //await userManager.AddClaimAsync(newUser, new Claim(type: "tags.view", "true"));

            //token
            return await GetAuthenticationResultAsync(newUser);
        }

        private async Task<AuthenticationResponse> GetAuthenticationResultAsync(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(options.SecretKey);


            var claims = new List<Claim>()
            {
                new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Email),
                new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email),
                new Claim(type: "Id", value: user.Id),
                new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
            };

            #region claim for jwt
            //kullanıcı kayıt olurken oluşturulan claimler db den çekilir
            //var userClaims = await userManager.GetClaimsAsync(user);
            //ve jwt payloadu içine bu claimler eklenir authorization için 
            //claims.AddRange(userClaims);
            #endregion

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
                var role = await roleManager.FindByNameAsync(userRole);
                if (role == null) continue;
                var roleClaims = await roleManager.GetClaimsAsync(role);

                foreach (var roleClaim in roleClaims)
                {
                    if (claims.Contains(roleClaim))
                        continue;

                    claims.Add(roleClaim);
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
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

            return new AuthenticationResponse
            {
                Token = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                Success = true,
                ErrorMessage = null

            };
        }
    }
}
