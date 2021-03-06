using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweet_Book.Authorization;
using Tweet_Book.Filters;
using Tweet_Book.Options;
using Tweet_Book.Services;

namespace Tweet_Book.Installers
{
    public class ApiInstaller : IInstaller
    {
        public void InstallerServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = new JwtSettings();
            configuration.Bind(nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);
            services.AddScoped<IIdentityService, IdentityService>();


            //services.AddControllersWithViews();
            //services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_3_0);
            //services.AddControllers(option => option.Filters.Add<ValidationFilter>())
            services.AddControllers()
               .AddFluentValidation(apiConfiguration => apiConfiguration.RegisterValidatorsFromAssemblyContaining<Startup>());

            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true


            };
            services.AddSingleton(tokenValidationParameters);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = tokenValidationParameters;
            });
            //services.AddAuthorization(options=>
            //{
            //    options.AddPolicy("TagViewer",builder=>builder.RequireClaim("tags.view","true"));
            //});
            services.AddAuthorization(options =>
             options.AddPolicy("MustWorkForChapsas", policy => {
                policy.AddRequirements(new WorksForCompanyRequirement("chapsas.com"));
            })
            );
            services.AddSingleton<IAuthorizationHandler, WorkForCompanyHandler>();
            services.AddSingleton<IUriService>(provider => 
            {
                var accessor = provider.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme,"://",request.Host.ToUriComponent(),"/");
                return new UriService(absoluteUri);

            });
            // swagger
            //services.AddHealthChecks();
        }
    }
}
