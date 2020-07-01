using Microsoft.EntityFrameworkCore;
using Reenbit.ChuckNorris.DataAccess.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reenbit.ChuckNorris.DataAccess
{
    class ReenbitChuckNorrisDbContext : DbContext
    {
        public ReenbitChuckNorrisDbContext(DbContextOptions<ReenbitChuckNorrisDbContext> options) : base(options) 
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new CategoryMap());
            modelBuilder.ApplyConfiguration(new JokeMap());
            modelBuilder.ApplyConfiguration(new JokeCategoryMap());
        }
    }
}
