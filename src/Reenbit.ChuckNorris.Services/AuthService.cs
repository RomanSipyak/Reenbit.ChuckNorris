using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Reenbit.ChuckNorris.Domain.DTOs;
using Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.UserDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Emails.Abstractions;
using Reenbit.ChuckNorris.Services.Abstraction;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Reenbit.ChuckNorris.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> signInManager;

        private readonly UserManager<User> userManager;

        private readonly IMapper mapper;

        private readonly IEmailService emailService;

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager, IMapper mapper, IEmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.mapper = mapper;
            this.emailService = emailService;
        }

        public async Task<SignInUserDto> SignInAsync(SignInCredentialsDto signInCredentialsDto)
        {
            var user = await this.userManager.FindByEmailAsync(signInCredentialsDto.Email);
            if (user != null)
            {
                var signInResult = await this.signInManager.PasswordSignInAsync(user, signInCredentialsDto.Password, false, false);
                if (signInResult.Succeeded)
                {
                    var roles = await this.userManager.GetRolesAsync(user);
                    var userDto = new SignInUserDto()
                    {
                        User = user,
                        Roles = roles.ToList()
                    };

                    return userDto;
                }
            }

            return null;
        }

        public ClaimsIdentity GetIdentity(SignInUserDto signInUserDto)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,signInUserDto.User.Email),
                new Claim(ClaimTypes.NameIdentifier, signInUserDto.User.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
            claimsIdentity.AddClaims(signInUserDto.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return claimsIdentity;
        }

        public async Task<bool> RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            var existingUser = await this.userManager.FindByEmailAsync(userRegisterDto.Email);
            if (existingUser != null)
            {
                return false;
            }

            var newUser = this.mapper.Map<User>(userRegisterDto);
            var createdUser = await this.userManager.CreateAsync(newUser, userRegisterDto.Password);
            if (userRegisterDto.Roles != null && userRegisterDto.Roles.Any())
            {
                //Can be invalid exception if role doesn't exist
                await this.userManager.AddToRolesAsync(newUser, userRegisterDto.Roles);
            }

            return createdUser.Succeeded;
        }

        public async Task<ActionExecutionResultDto> ResetPasswordRequestAsync(ResetPasswordRequestDto resetPasswordRequest)
        {
            var result = new ActionExecutionResultDto();
            var user = await this.userManager.FindByEmailAsync(resetPasswordRequest.Email);
            if (user != null)
            {
                var resetToken = await this.userManager.GeneratePasswordResetTokenAsync(user);
                await this.emailService.SendRestorePasswordEmail(user, resetToken, resetPasswordRequest.ResetPageUrl);
            }
            else
            {
                result.Succeeded = false;
                result.Error = "User was not found";
            }

            return result;
        }

        public async Task<ActionExecutionResultDto> ChangePassword(ResetPasswordDto resetPasswordDto)
        {
            var actionResult = new ActionExecutionResultDto();
            var user = await this.userManager.FindByIdAsync(resetPasswordDto.UserId.ToString());
            IdentityResult changePasswordResult = null;
            if (user != null)
            {
                changePasswordResult = await this.userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            }

            if (changePasswordResult == null || !changePasswordResult.Succeeded)
            {
                actionResult.Succeeded = false;
                actionResult.Error = changePasswordResult?.Errors.Select(r => r.Description).FirstOrDefault() ?? "User was not found";
            }

            return actionResult;
        }

        public async Task<bool> VerifyResetPasswordToken(ValidateResetPasswordDto validateResetPasswordDto)
        {
            bool result = false;
            var user = await this.userManager.FindByIdAsync(validateResetPasswordDto.UserId.ToString());
            if (user != null)
            {
                result = await this.userManager.VerifyUserTokenAsync(
                    user,
                    this.userManager.Options.Tokens.PasswordResetTokenProvider,
                    UserManager<User>.ResetPasswordTokenPurpose,
                    validateResetPasswordDto.Token);
            }

            return result;
        }
    }
}
