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
