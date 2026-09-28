using Microsoft.EntityFrameworkCore;
using PetCare.API.Models;

namespace PetCare.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<PetTreatment> PetTreatments { get; set; }
        public DbSet<PetTreatmentHistory> PetTreatmentHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure unique constraint for Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure one-to-many relationship User -> Treatments
            modelBuilder.Entity<Treatment>()
                .HasOne(t => t.User)
                .WithMany(u => u.Treatments)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
