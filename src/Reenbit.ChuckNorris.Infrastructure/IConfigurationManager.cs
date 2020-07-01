using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Infrastructure
{
    public interface IConfigurationManager
    {
        string DatabaseConnectionStrings { get; }
    }
}
