using Microsoft.Extensions.Configuration;

namespace Reenbit.ChuckNorris.Infrastructure
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration configuration;

        public string DatabaseConnectionString => GetConnectionStringValue("DatabaseConnectionString");

        public ConfigurationManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string GetConnectionStringValue(string connectionName)
        {
            return this.configuration.GetConnectionString(connectionName);
        }
    }
}
