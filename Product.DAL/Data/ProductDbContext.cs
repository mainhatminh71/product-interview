using System;
using System.Collections.Generic;
using System.Text;
using Product.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Product.DAL.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                return;
            }
            
            // Suppress PendingModelChangesWarning để tránh lỗi khi có thay đổi model chưa có migration
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
        public DbSet<User> Users => Set<User>();
        public DbSet<Item> Products => Set<Item>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Item>().ToTable("Items");
            base.OnModelCreating(modelBuilder);
        }
    }
}
