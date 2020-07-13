using Newtonsoft.Json;
using System.Collections.Generic;

namespace Reenbit.ChuckNorris.Domain.DTOs.JokeDTOS
{
    public class JokeDto
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string Url { get; set; }

        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        public ICollection<string> Categories { get; set; }
    }
}
