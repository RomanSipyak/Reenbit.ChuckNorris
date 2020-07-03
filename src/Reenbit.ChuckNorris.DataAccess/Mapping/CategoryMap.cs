using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.HasMany(c => c.JokeCategories)
                   .WithOne(jc => jc.Category)
                   .HasForeignKey(jc => jc.CategoryId);

            builder.Property(c => c.Title)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}
