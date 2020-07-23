using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS
{
    public class CreateJokeDto
    {
        [Required]
        public string Value { get; set; }

        public ICollection<int> Categories { get; set; } = new List<int>();

        public ICollection<string> ImageNames { get; set; } = new List<string>();
    }
}
