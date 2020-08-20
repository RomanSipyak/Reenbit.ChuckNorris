using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Reenbit.ChuckNorris.API.Authentication;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.UserDTOS;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Web;

namespace Reenbit.ChuckNorris.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthCotroller : ControllerBase
    {
        private const string WrongSignIn = "Email or password is incorrect";

        private readonly IAuthService authService;

        private readonly IMapper mapper;

        private readonly IWebHostEnvironment hostingEnvironment;

        private readonly ILogger<AuthCotroller> logger;

        public AuthCotroller(IAuthService authService, IMapper mapper, IWebHostEnvironment hostingEnvironment, ILogger<AuthCotroller> logger)
        {
            this.authService = authService;
            this.mapper = mapper;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        [HttpPost, AllowAnonymous]
        [Route("signin")]
        public async Task<IActionResult> SignIn([FromBody]SignInCredentialsDto signInCredential)
        {
            var user = await this.authService.SignInAsync(signInCredential);
            if (user != null)
            {
                string token = this.GenerateToken(user);
                return Ok(new
                {
                    token,
                    user = this.mapper.Map<UserDto>(user)
                });
            }
            else
            {
                return BadRequest(WrongSignIn);
            }
        }

        [HttpPost, AllowAnonymous]
        [Route("signup")]
        public async Task<IActionResult> SignUp([FromBody]UserRegisterDto userRegisterDto)
        {
            bool result = await this.authService.RegisterUserAsync(userRegisterDto);
            return Ok(result);
        }

        [HttpPost, AllowAnonymous]
        [Route("resetPasswordRequest")]
        public async Task<ActionResult<ActionExecutionResultDto>> ResetPasswordRequestAsync([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var result = await authService.ResetPasswordRequestAsync(resetPasswordRequestDto);
            return result.Succeeded ? Ok(result) : (ActionResult)BadRequest(result.Error);
        }

        [Route("verifyResetPasswordToken")]
        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> VerifyResetPasswordToken([FromQuery]ValidateResetPasswordDto validateResetPasswordDto)
        {
            validateResetPasswordDto.Token = validateResetPasswordDto.Token.Replace(" ", "+");
            var result = await this.authService.VerifyResetPasswordToken(validateResetPasswordDto);
            return Ok(result);
        }

        [Route("changePassword")]
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ChangePassword([FromBody]ResetPasswordDto resetPasswordDto)
        {
            var result = await this.authService.ChangePassword(resetPasswordDto);
            return result.Succeeded ? Ok(result) : (IActionResult)BadRequest(result.Error);
        }

        private string GenerateToken(SignInUserDto user)
        {
            var identity = this.authService.GetIdentity(user);
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: DateTime.UtcNow,
                    claims: identity.Claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}
