using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS
{
    public class ValidateResetPasswordDto
    {
        public int UserId { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
