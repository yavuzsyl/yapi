using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
            var jwtSettings = new JwtSettings();
            configuration.Bind(key: nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddScoped<IIdentityService, IdentityService>();


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

            //custom authorization requirement
            services.AddAuthorization(options =>
            {
                options.AddPolicy("WorksForDude", configurePolicy: policy =>
                  {
                      policy.AddRequirements(new WorksForCompanyRequirement("dude.com"));
                      //policy.RequireRole("Poster", "Admin");
                  });
            });



            services.AddSingleton<IAuthorizationHandler, WorksForCompanyHandler>();

            //policy combintaion of the rules accessing something in the system
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy(name: "TagViewer", configurePolicy: builder => builder.RequireClaim("tags.view", allowedValues: "true"));
            //});gonna use roles instead of claim

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add<ValidationFilter>();//validation filter
           
            }).AddFluentValidation(mvcConfiguration => mvcConfiguration
                .RegisterValidatorsFromAssemblyContaining<Startup>())//going to register AbstractValidator validators
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

           
        }
    }
}
