using Microsoft.AspNetCore.Http;
using Reenbit.ChuckNorris.Domain.DTOs.ImageDTOS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IMediaService
    {
        Task<UploadImageDto> GenerateSasTokenWithPermissioWrite(string fileExtencion, string containerName);

        Task<string> CopyFile(string sourceName, string destinationName, string containerSourceName, string containerDestinationName);
    }
}
