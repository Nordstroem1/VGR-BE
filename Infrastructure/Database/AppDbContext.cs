using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Article> Articles => Set<Article>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MaterialType).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

    }
}
