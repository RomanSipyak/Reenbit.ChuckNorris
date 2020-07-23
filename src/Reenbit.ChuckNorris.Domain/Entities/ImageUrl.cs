using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class ImageUrl
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public int JokeId { get; set; }

        public Joke Joke { get; set; }
    }
}
