using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class UserFavoriteMap : IEntityTypeConfiguration<UserFavorite>
    {
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            builder.ToTable("UserFavorites");

            builder.HasKey(uf => new { uf.UserId, uf.JokeId });

            builder.Property(uf => uf.JokeId).IsRequired();

            builder.Property(uf => uf.UserId).IsRequired();
        }
    }
}
