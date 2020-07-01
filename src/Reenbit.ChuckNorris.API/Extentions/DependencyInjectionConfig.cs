using Microsoft.Extensions.DependencyInjection;
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

            return services;
        }


        public static void RegisterDataAccess(this IServiceCollection services)
        {
        // add uow    
        }

        public static void RegisterInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IConfigurationManager, ConfigurationManager>();
        }

    }
}
