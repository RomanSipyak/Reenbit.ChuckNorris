using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS
{
    public class CreateJokeDTO
    {
        [Required]
        public string Value { get; set; }

        public string Url { get; set; }
       
        public string IconUrl { get; set; }

        public ICollection<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }
}
