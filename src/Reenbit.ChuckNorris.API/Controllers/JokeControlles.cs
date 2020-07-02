using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.Controllers
{
    [ApiController]
    [Route("api/jokes")]
    public class JokeControlles : ControllerBase
    {
        private readonly IJokeService jokeService;

        public JokeControlles(IJokeService jokeService)
        {
            this.jokeService = jokeService;
        }
        //TODO RefactorStatusCodes
        [HttpGet]
        [Route("random")]
        public async Task<IActionResult> GetRandomJoke([FromQuery][ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> category, [FromQuery] string query)
        {
            return Ok(await jokeService.GetRundomJokeAsync(category, query));
        }

        }
    }
}
