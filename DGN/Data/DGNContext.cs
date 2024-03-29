﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DGN.Models;

namespace DGN.Data
{
    public class DGNContext : DbContext
    {
        public DGNContext (DbContextOptions<DGNContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Article>()
                .HasIndex(a => a.Title)
                .IsUnique();
        }

        public DbSet<DGN.Models.Article> Article { get; set; }

        public DbSet<DGN.Models.Category> Category { get; set; }

        public DbSet<DGN.Models.Comment> Comment { get; set; }

        public DbSet<DGN.Models.Password> Password { get; set; }

        public DbSet<DGN.Models.User> User { get; set; }
        
        public DbSet<DGN.Models.Branch> Branch { get; set; }
    }
}
