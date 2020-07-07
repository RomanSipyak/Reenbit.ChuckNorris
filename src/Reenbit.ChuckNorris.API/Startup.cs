using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Reenbit.ChuckNorris.API.CustomMiddlewares;
using Reenbit.ChuckNorris.API.Extentions;
using Reenbit.ChuckNorris.DataAccess;
using Reenbit.ChuckNorris.Domain.DTOsProfiles;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Infrastructure;

namespace Reenbit.ChuckNorris.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterDependencies();
            services.AddCors(options => options.AddPolicy("CorsPolicy", builder => builder.AllowAnyOrigin()
                                                                                          .AllowAnyMethod()
                                                                                          .AllowAnyHeader()));
            services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfileForDTOs)));

            var configurationManager = GetConfigurationManager(services);
            services.AddDbContext<ReenbitChuckNorrisDbContext>(options =>
                                                               options.UseSqlServer(configurationManager.DatabaseConnectionString));

            services.AddIdentity<User, Role>(options =>
            options.Password = new PasswordOptions
            {
                RequireDigit = false,
                RequiredLength = 6,
                RequireLowercase = false,
                RequireUppercase = false,
                RequireNonAlphanumeric = false
            }).AddEntityFrameworkStores<ReenbitChuckNorrisDbContext>()
              .AddDefaultTokenProviders();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CHUCKNORRIS API", Version = "v1" });
            });

            services.AddControllers(setupActions =>
            {
                setupActions.ReturnHttpNotAcceptable = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<ErrorHandlingExceptionsMiddleware>();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CHUCKNORRIS API");
            });

            app.UseCors("CorsPolicy");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IConfigurationManager GetConfigurationManager(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            IConfigurationManager config = serviceProvider.GetService<IConfigurationManager>();
            return config;
        }
    }
}
