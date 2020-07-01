using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    public class JokeMap : IEntityTypeConfiguration<Joke>
    {
        public void Configure(EntityTypeBuilder<Joke> builder)
        {
            builder.ToTable("Jokes");

            builder.HasKey(jk => jk.Id);

            builder.HasMany(jk => jk.JokeCategories)
                   .WithOne(jct => jct.Joke)
                   .HasForeignKey(jct => jct.JokeId);

            builder.Property(jc => jc.Value)
                   .IsRequired()
                   .HasMaxLength(2000);
        }
    }
}
