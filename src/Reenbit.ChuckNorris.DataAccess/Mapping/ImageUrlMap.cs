using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class ImageUrlMap : IEntityTypeConfiguration<ImageUrl>
    {
        public void Configure(EntityTypeBuilder<ImageUrl> builder)
        {
            builder.ToTable("ImageUrls");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Value)
                   .IsRequired()
                   .HasMaxLength(2048);

            builder.Property(i => i.JokeId)
                   .IsRequired();
        }
    }
}
