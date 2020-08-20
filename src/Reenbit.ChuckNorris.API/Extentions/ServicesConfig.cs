using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Reenbit.ChuckNorris.API.Authentication;
using Reenbit.ChuckNorris.DataAccess;
using Reenbit.ChuckNorris.Domain.ConfigClasses;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Extentions
{
    public static class ServicesConfig
    {
        public static void AddJwtBearerConfig(this IServiceCollection services)
        {
            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,

                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,

                        ValidAudience = AuthOptions.AUDIENCE,

                        ValidateLifetime = true,

                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

                        ValidateIssuerSigningKey = true,
                    };
                });
        }

        public static void AddIdetityConfig(this IServiceCollection services)
        {
            services.AddIdentity<User, Role>(options => options.Password = new PasswordOptions
            {
                RequireDigit = false,
                RequiredLength = 6,
                RequireLowercase = false,
                RequireUppercase = false,
                RequireNonAlphanumeric = false
            }).AddEntityFrameworkStores<ReenbitChuckNorrisDbContext>().AddDefaultTokenProviders();
        }

        public static void AddAzureStorageBlobOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureStorageBlobOptions>(configuration.GetSection("AzureStorageBlobOptions"));
        }

        public static void AddEmailSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
        }
    }
}
