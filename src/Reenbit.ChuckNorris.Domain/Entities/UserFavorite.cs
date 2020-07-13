using System;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class UserFavorite
    {
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public int JokeId { get; set; }

        public Joke Joke { get; set; }
    }
}
