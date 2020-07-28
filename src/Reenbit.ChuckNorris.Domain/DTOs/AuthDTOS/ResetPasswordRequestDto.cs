using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.AuthDTOS
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ResetPageUrl { get; set; }
    }
}
