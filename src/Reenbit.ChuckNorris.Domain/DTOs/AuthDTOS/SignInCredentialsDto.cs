using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS
{
    public class SignInCredentialsDto
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
