using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
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

        public MediaController(IMediaService mediaService)
        {
            this.mediaService = mediaService;
        }

        [HttpGet]
        [Route("uploadSas")]
        public async Task<IActionResult> GetSasKey([FromQuery] string fileName)
        {
            return Ok(mediaService.GenerateSasTokenWithPermissioWriteInTemp(fileName));
        }
    }
}
