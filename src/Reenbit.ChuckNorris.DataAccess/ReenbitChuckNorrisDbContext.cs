using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reenbit.ChuckNorris.DataAccess.Mapping;
using Reenbit.ChuckNorris.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess
{
    public class ReenbitChuckNorrisDbContext : IdentityDbContext<User, Role, int,
                                                          IdentityUserClaim<int>, UserRole,
                                                          IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public ReenbitChuckNorrisDbContext(DbContextOptions<ReenbitChuckNorrisDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new RoleMap());
            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new JokeMap());
            modelBuilder.ApplyConfiguration(new UserJokeMap());
            modelBuilder.ApplyConfiguration(new JokeCategoryMap());
        }
    }
}
