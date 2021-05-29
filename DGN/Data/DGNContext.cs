using System;
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

        public DbSet<DGN.Models.Branch> Branch { get; set; }
    }
}
