using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Reenbit.ChuckNorris.Domain.DTOs.CategoryDTOS;
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
        [JsonProperty("icon_url")]
        public string IconUrl { get; set; }

        public ICollection<string> Categories { get; set; }
    }
}
