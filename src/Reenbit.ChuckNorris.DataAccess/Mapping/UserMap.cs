using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess.Mapping
{
    class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u => u.Claims)
                   .WithOne()
                   .HasForeignKey(uc => uc.UserId)
                   .IsRequired();

            builder.HasMany(u => u.Logins)
                   .WithOne()
                   .HasForeignKey(ul => ul.UserId)
                   .IsRequired();

            builder.HasMany(u => u.Tokens)
                   .WithOne()
                   .HasForeignKey(ut => ut.UserId)
                   .IsRequired();

            builder.HasMany(u => u.UserRoles)
                   .WithOne()
                   .HasForeignKey(ur => ur.UserId)
                   .IsRequired();

            builder.HasMany(u => u.UserFavorites)
                   .WithOne(uf => uf.User)
                   .HasForeignKey(uf => uf.UserId)
                   .IsRequired();
        }
    }
}
