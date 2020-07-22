using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IMediaService
    {
        string GenerateFeSasToken(string fileName);
        Task CopyImageFromTempToPermanentContainer(string sourceName, string destinationName);
    }
}
