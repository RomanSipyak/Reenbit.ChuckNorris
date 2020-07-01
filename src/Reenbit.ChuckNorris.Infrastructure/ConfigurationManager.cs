﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Infrastructure
{
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly IConfiguration configuration;

        public string DatabaseConnectionStrings => GetConnectionStringValue("DatabaseConnectionString");

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