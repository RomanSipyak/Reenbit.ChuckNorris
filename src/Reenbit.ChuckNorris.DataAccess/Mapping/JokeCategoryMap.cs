using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class JokeCategoryMap : IEntityTypeConfiguration<JokeCategory>
    {
        public void Configure(EntityTypeBuilder<JokeCategory> builder)
        {
            builder.ToTable("JokeCategories");

            builder.HasKey(jc => new { jc.JokeId, jc.CategoryId });

            builder.Property(jc => jc.JokeId).IsRequired();

            builder.Property(jc => jc.CategoryId).IsRequired();
        }
    }
}
