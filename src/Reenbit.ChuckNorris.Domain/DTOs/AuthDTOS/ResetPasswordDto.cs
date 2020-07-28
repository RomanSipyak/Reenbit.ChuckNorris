using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS
{
    public class ResetPasswordDto
    {
        public int UserId { get; set; }

        public string Token { get; set; }

        public string Password { get; set; }
    }
}
