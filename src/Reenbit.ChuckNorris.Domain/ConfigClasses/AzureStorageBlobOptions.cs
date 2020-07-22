using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Domain.ConfigClasses
{
    public class AzureStorageBlobOptions
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string ConnectionString { get; set; }
        public string FileTempPath { get; set; }
        public string FilePath { get; set; }
    }
}
