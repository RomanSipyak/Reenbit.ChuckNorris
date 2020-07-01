using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    public class CategoryMap : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            builder.HasKey(ct => ct.Id);

            builder.HasMany(ct => ct.JokeCategories)
                   .WithOne(jct => jct.Category)
                   .HasForeignKey(jct => jct.CategoryId);

            builder.Property(ct => ct.Title)
                   .IsRequired()
                   .HasMaxLength(50);
        }
    }
}
