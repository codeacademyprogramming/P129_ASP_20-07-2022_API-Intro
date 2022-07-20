using Microsoft.EntityFrameworkCore;
using P129FirstApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P129FirstApi.Data
{
    public class AppDbContext : DbContext
    {
        //Migration Commands
        //Add-Migration InitialCreate -OutputDir Data\Migrations
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
    }
}
