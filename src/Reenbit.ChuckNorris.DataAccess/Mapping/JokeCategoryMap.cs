using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class JokeCategoryMap : IEntityTypeConfiguration<JokeCategory>
    {
        public void Configure(EntityTypeBuilder<JokeCategory> builder)
        {
            builder.ToTable("JokeCategory");

            builder.HasKey(jct => new { jct.JokeId, jct.CategoryId});

            builder.Property(jct => jct.JokeId).IsRequired();
            builder.Property(jct => jct.CategoryId).IsRequired();
        }
    }
}
