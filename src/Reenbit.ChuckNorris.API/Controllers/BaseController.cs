using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Controllers
{
    public class BaseApiController : ControllerBase
    {
        public string UserId => GetCurrentUserId();

        protected string GetCurrentUserId()
        {
            var userId = this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            return userId;
        }
    }
}
