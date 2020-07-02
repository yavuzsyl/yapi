using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YAPI.CustomAuthorization;
using YAPI.Domain;
using YAPI.Filters;
using YAPI.Options;
using YAPI.Services;

namespace YAPI.Installers
{
    public class MvcInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            #region settings
            var jwtSettings = new JwtSettings();
            configuration.Bind(key: nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            #endregion

            services.AddScoped<IIdentityService, IdentityService>();

            #region CORS
            services.AddCors(c =>
            {
                c.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:5000")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            }
            );
            #endregion

            #region token validation parameters
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            //to reach this added as singleton service to service container
            services.AddSingleton(tokenValidationParameters);
            #endregion

            services.AddAuthentication(configureOptions: x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;

            });

            #region custom authorization with policies
            services.AddAuthorization(options =>
            {
                //custom policy eklenir ve daha sonra istenilen ep de kullanılır
                options.AddPolicy("WorksForDude", configurePolicy: policy =>
                  {
                      policy.AddRequirements(new WorksForCompanyRequirement("dude.com"));
                      //policy.RequireRole("Poster", "Admin");
                  });
            });
            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();

            //policy combintaion of the rules accessing something in the system
            //services.AddAuthorization(options =>
            //{   //burada gelen kullanıcının token payloadundaki claimlerinde tags.view true olarak varsa bu policy uygulanan epleri consume edebilir.Bunun için token oluşturuken claimlere ekleme yapılması gerekir 
            //    options.AddPolicy(name: "TagViewer", configurePolicy: builder => builder.RequireClaim("tags.view", allowedValues: "true"));
            //});gonna use roles instead of claim
            #endregion


            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<ValidationFilter>();//validation filter for modelstate control

            }).AddFluentValidation(mvcConfiguration => mvcConfiguration
                .RegisterValidatorsFromAssemblyContaining<Startup>())//going to register AbstractValidator validators
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            //service containera eklendi baseuri requestten alınacak
            services.AddSingleton<IUriService>(provider =>
            {  
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri);
            });

        }
    }
}
