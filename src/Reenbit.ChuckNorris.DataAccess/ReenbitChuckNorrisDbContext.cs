using Microsoft.EntityFrameworkCore;
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
        }
    }
}
