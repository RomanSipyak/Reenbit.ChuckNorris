using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.UserDTOS;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services.Abstraction
{
    public interface IAuthService
    {
        Task<SignInUserDto> SignInAsync(SignInCredentialsDto signInCredentialsDto);

        ClaimsIdentity GetIdentity(SignInUserDto signInUserDto);

        Task<bool> RegisterUserAsync(UserRegisterDto userRegisterDto);

        Task<ActionExecutionResultDto> ResetPasswordRequestAsync(ResetPasswordRequestDto resetPasswordRequestDto); 
    }
}
