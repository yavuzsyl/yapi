using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using YAPI.Domain;
using YAPI.Options;

namespace YAPI.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly JwtSettings options;

        public IdentityService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IOptions<JwtSettings> options)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.options = options.Value;
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

            var createdUser = await userManager.CreateAsync(newUser,password);//gonaa hash this pass mofo
            if (!createdUser.Succeeded)
                return new AuthenticationResult { ErrorMessage = createdUser.Errors.Select(x => x.Description) };

            //token
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(options.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type:JwtRegisteredClaimNames.Sub, value : newUser.Email),
                    new Claim(type:JwtRegisteredClaimNames.Email, value : newUser.Email),
                    new Claim(type:"Id", value : newUser.Id),
                    new Claim(type:JwtRegisteredClaimNames.Jti, value : Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey),algorithm: SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationResult
            {
                Token = tokenHandler.WriteToken(token),
                Success = true,
                ErrorMessage = null
                
            };
        }
    }
}
