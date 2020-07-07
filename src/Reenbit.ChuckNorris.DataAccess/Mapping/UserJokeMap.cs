using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class UserJokeMap : IEntityTypeConfiguration<UserJoke>
    {
        public void Configure(EntityTypeBuilder<UserJoke> builder)
        {
            builder.ToTable("UserJoke");

            builder.HasKey(uj => new { uj.UserId, uj.JokeId });

            builder.Property(uj => uj.UserId).IsRequired();

            builder.Property(uj => uj.JokeId).IsRequired();

            builder.Property(uj => uj.Favourite).HasDefaultValue(false);
        }
    }
}
