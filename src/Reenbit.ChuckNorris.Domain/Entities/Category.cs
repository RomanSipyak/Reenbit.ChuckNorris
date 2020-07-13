using System.Collections.Generic;

namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public ICollection<JokeCategory> JokeCategories { get; set; } = new List<JokeCategory>();
    }
}
