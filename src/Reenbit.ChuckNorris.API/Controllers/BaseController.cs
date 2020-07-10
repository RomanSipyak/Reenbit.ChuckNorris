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
        public int UserId => GetCurrentUserId();

        protected int GetCurrentUserId()
        {
            var userId = Int32.Parse(this.User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return userId;
        }
    }
}
