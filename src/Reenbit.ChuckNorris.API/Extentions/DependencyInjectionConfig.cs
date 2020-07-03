using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Reenbit.ChuckNorris.DataAccess;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.DataAccess.Abstraction.Repositories;
using Reenbit.ChuckNorris.DataAccess.Repositories;
using Reenbit.ChuckNorris.Domain.DTOsProfiles;
using Reenbit.ChuckNorris.Infrastructure;
using Reenbit.ChuckNorris.Services;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Extentions
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            RegisterInfrastructure(services);
            RegisterServices(services);
            RegisterDataAccess(services);
            return services;
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<IJokeService, JokeService>();
            services.AddTransient<ICategoryService, CategoryService>();
        }

        public static void RegisterDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IJokeRepository, JokeRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
        }

        public static void RegisterInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IConfigurationManager, ConfigurationManager>();
        }
    }
}
