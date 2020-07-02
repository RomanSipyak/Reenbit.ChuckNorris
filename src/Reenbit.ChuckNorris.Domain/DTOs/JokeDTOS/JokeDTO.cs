using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Reenbit.ChuckNorris.Domain.DTOs
{
    public class JokeDTO
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string Url { get; set; }
        [JsonProperty("icon_url")]//TODO need to be fixed (read abour reflection and config mapping profiles better)
        public string IconUrl { get; set; }

       // public ICollection<JokeCategory> JokeCategories { get; set; }
    }
}
