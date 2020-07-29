using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class JokeMap : IEntityTypeConfiguration<Joke>
    {
        public void Configure(EntityTypeBuilder<Joke> builder)
        {
            builder.ToTable("Jokes");

            builder.HasKey(j => j.Id);

            builder.HasMany(j => j.JokeCategories)
                   .WithOne(jc => jc.Joke)
                   .HasForeignKey(jc => jc.JokeId);

            builder.Property(j => j.Value)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.HasMany(j => j.UserFavorites)
                   .WithOne(uf => uf.Joke)
                   .HasForeignKey(uf => uf.JokeId)
                   .IsRequired();

            builder.HasMany(j => j.JokeImages)
                   .WithOne(i => i.Joke)
                   .HasForeignKey(i => i.JokeId)
                   .IsRequired();
        }
    }
}
