using Microsoft.Extensions.DependencyInjection;
using Reenbit.ChuckNorris.DataAccess;
using Reenbit.ChuckNorris.DataAccess.Abstraction;
using Reenbit.ChuckNorris.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Extentions
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterDependencies(this IServiceCollection services)
        {
            RegisterInfrastructure(services);
            RegisterDataAccess(services);
            return services;
        }


        public static void RegisterDataAccess(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
        }

        public static void RegisterInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IConfigurationManager, ConfigurationManager>();
        }

    }
}
