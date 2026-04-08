using Microsoft.EntityFrameworkCore;
using Todolist.Models;

namespace Todolist.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TodoItem> Todos { get; set; }
        public DbSet<Profile> Profiles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=todos.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Login).IsRequired().HasMaxLength(64);
                entity.HasIndex(p => p.Login).IsUnique();
                entity.Property(p => p.Password).IsRequired().HasMaxLength(128);
                entity.Property(p => p.FirstName).IsRequired().HasMaxLength(64);
                entity.Property(p => p.LastName).IsRequired().HasMaxLength(64);
                entity.Property(p => p.BirthYear).IsRequired();
            });

            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Text).IsRequired().HasMaxLength(2000);
                entity.Property(t => t.Status).IsRequired();
                entity.Property(t => t.LastUpdate).IsRequired();
                entity.Property(t => t.SortOrder).IsRequired();

                entity.HasOne(t => t.Profile)
                    .WithMany(p => p.Todos)
                    .HasForeignKey(t => t.ProfileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => new { t.ProfileId, t.SortOrder });
            });
        }
    }
}