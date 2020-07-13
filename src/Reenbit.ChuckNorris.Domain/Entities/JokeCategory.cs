namespace Reenbit.ChuckNorris.Domain.Entities
{
    public class JokeCategory
    {
        public int JokeId { get; set; }

        public Joke Joke { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
