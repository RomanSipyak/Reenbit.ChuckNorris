using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS
{
    public class JokeDto
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string Url { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public ICollection<string> Categories { get; set; }

        public ICollection<string> ImageUrls { get; set; }
    }
}
