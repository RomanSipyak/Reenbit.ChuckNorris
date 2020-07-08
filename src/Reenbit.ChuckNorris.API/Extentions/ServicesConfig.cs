using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Reenbit.ChuckNorris.API.Authentication;
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
    }
}
