using Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.UserDTOS;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IAuthService
    {
        public Task<SignInUserDto> SignInAsync(SignInCredentialsDto signInCredentialsDto);

        public ClaimsIdentity GetIdentity(SignInUserDto signInUserDto);
        public Task RegisterUserAsync(UserRegisterDto userRegisterDto);
    }
}
