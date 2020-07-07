using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class Joke
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public string Url { get; set; }

        public string IconUrl { get; set; }

        public ICollection<JokeCategory> JokeCategories { get; set; } = new List<JokeCategory>();

        public ICollection<UserJoke> UserJokes { get; set; } = new List<UserJoke>();
    }
}
