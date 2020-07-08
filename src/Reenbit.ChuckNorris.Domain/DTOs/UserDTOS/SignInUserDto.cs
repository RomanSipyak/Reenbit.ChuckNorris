using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.UserDTOS
{
    public class SignInUserDto
    {
        public User User { get; set; }

        public List<string> Roles { get; set; }
    }
}
