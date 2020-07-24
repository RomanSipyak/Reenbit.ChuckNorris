using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class ImageMap : IEntityTypeConfiguration<JokeImage>
    {
        public void Configure(EntityTypeBuilder<JokeImage> builder)
        {
            builder.ToTable("JokeImages");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Url)
                   .IsRequired()
                   .HasMaxLength(2048);

            builder.Property(i => i.JokeId)
                   .IsRequired();
        }
    }
}
