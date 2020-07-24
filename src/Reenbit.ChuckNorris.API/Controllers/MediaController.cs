using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Options;
using Reenbit.ChuckNorris.Domain.ConfigClasses;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Controllers
{
    [ApiController]
    [Route("api/media")]
    public class MediaController : BaseApiController
    {
        private readonly IMediaService mediaService;

        private readonly IOptions<AzureStorageBlobOptions> azureStorageBlobOptions;

        public MediaController(IMediaService mediaService, IOptions<AzureStorageBlobOptions> azureStorageBlobOptions)
        {
            this.mediaService = mediaService;
            this.azureStorageBlobOptions = azureStorageBlobOptions;
        }

        [HttpPost]
        public async Task<IActionResult> GetSasKey([FromQuery] string fileExtencion)
        {
            var uploadImageDto = await mediaService.GenerateSasTokenWithPermissionWrite(fileExtencion, azureStorageBlobOptions.Value.FileTempPath);
            return CreatedAtAction(nameof(GetSasKey), uploadImageDto);
        }
    }
}
