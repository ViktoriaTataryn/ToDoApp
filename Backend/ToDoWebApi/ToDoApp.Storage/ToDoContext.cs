using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Entities.Models;

namespace ToDoApp.Storage
{
    public class ToDoContext : DbContext
    {
        public ToDoContext() { }

        public ToDoContext(DbContextOptions<ToDoContext> options) :base (options) { 

        }
     
        public DbSet<User> Users { get; set; }
        public DbSet<ToDoTask> Tasks { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зв'язку: User -> Tasks
            modelBuilder.Entity<ToDoTask>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Налаштування зв'язку: User -> Categories
            modelBuilder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Налаштування зв'язку: Category -> Tasks
            modelBuilder.Entity<ToDoTask>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.SetNull); // Категорію видалили -> таска залишилась без категорії
        }
    }
}
