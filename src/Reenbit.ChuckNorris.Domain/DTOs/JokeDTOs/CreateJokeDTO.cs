using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS
{
    public class CreateJokeDto
    {
        [Required]
        public string Value { get; set; }

        public string Url { get; set; }
       
        public string IconUrl { get; set; }

        public ICollection<int> Categories { get; set; } = new List<int>();
    }
}
