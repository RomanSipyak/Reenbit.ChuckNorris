﻿namespace Reenbit.ChuckNorris.Infrastructure
{
    public interface IConfigurationManager
    {
        string DatabaseConnectionString { get; }
    }
}
