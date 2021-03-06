using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbContents
{
    public class MyDbContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<House> Houses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql("Host=127.0.0.1;Port=5432;Username=postgres;Password=ca7d98cc1ea815063; Database=Origin;Pooling=true;Minimum Pool Size=1;");
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseBatchEF_Npgsql();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //从当前程序集加载所有的IEntityTypeConfiguration
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }


    }
}
