using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS;
using Reenbit.ChuckNorris.Domain.DTOs.UserDTOS;
using Reenbit.ChuckNorris.Domain.Entities;
using Reenbit.ChuckNorris.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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

        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager, IMapper mapper)
        {
            this.mapper = mapper;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<SignInUserDto> SignInAsync(SignInCredentialsDto signInCredentialsDto)
        {
            var user = await this.userManager.FindByEmailAsync(signInCredentialsDto.Email);
            if (user != null)
            {
                var signInResult = await this.signInManager.PasswordSignInAsync(user, signInCredentialsDto.Password, signInCredentialsDto.RememberMe, false);
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

        public async Task RegisterUserAsync(UserRegisterDto userRegisterDto)
        {
            var existingUser = await this.userManager.FindByEmailAsync(userRegisterDto.Email);
            if (existingUser != null)
            {
                return;
            }

            var newUser = this.mapper.Map<User>(userRegisterDto);
            var createdUser = await this.userManager.CreateAsync(newUser, userRegisterDto.Password);
            return;
        }
    }
}
