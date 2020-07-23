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
        UploadImageDto GenerateSasTokenWithPermissioWriteInTemp(string fileName);
        Task<string> CopyImageFromTempToPermanentContainer(string sourceName, string destinationName);
    }
}
