using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS
{
    public class UpdateJokeDto
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public string IconUrl { get; set; }

        public ICollection<int> Categories { get; set; } = new List<int>();

        public ICollection<string> NewImagesNames { get; set; } = new List<string>();

        public ICollection<string> OldImagesLinks { get; set; } = new List<string>();
    }
}
