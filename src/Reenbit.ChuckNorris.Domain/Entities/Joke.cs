using System.Collections.Generic;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class Joke : TrackedEntity
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public ICollection<JokeCategory> JokeCategories { get; set; } = new List<JokeCategory>();

        public virtual ICollection<UserFavorite> UserFavorites { get; set; } = new List<UserFavorite>();

        public virtual ICollection<JokeImage> JokeImages { get; set; } = new List<JokeImage>();
    }
}
