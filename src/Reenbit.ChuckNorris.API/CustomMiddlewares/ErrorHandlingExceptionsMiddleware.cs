using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Reenbit.ChuckNorris.Services.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.API.CustomMiddlewares
{
    public class ErrorHandlingExceptionsMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingExceptionsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private Task HandleException(HttpContext context, Exception excption)
        {
            var code = HttpStatusCode.InternalServerError;

            if (excption is CategoryNotFoundException || excption is SearchQueryException)
            {
                code = HttpStatusCode.BadRequest;
            }
            else if (excption is Exception)
            {
                code = HttpStatusCode.InternalServerError;
            }

            var result = JsonConvert.SerializeObject(new { error = excption.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
