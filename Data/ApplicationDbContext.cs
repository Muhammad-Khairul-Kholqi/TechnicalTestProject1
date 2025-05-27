using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using technicalTestProject_1.Models;

namespace technicalTestProject_1.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Category>()
                .HasOne(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Category>().ToTable("Categories");

            builder.Entity<Product>()
                .HasOne(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product>().ToTable("Products");

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FullName)
                    .HasMaxLength(100)
                    .IsRequired(false);

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.IsEmailConfirmed)
                    .HasDefaultValue(false);
            });
        }
    }
}