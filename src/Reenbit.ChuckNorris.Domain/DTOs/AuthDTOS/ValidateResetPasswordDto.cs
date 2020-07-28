using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS
{
    public class ValidateResetPasswordDto
    {
        public int UserId { get; set; }

        public string Token { get; set; }
    }
}
