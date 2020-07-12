using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace Reenbit.ChuckNorris.API.Controllers
{
    public class BaseApiController : ControllerBase
    {
        public int UserId => GetCurrentUserId();

        protected int GetCurrentUserId()
        {
            var userId = Int32.Parse(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return userId;
        }
    }
}
