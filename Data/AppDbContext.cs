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
        
        public DbSet<EventTypeDefinition> EventTypeDefinitions { get; set; }
        public DbSet<HealthEvent> HealthEvents { get; set; }
        public DbSet<HealthEventOccurrence> HealthEventOccurrences { get; set; }
        public DbSet<VetVisitEvent> VetVisitEvents { get; set; }
        public DbSet<MedicationEvent> MedicationEvents { get; set; }
        public DbSet<VaccineEvent> VaccineEvents { get; set; }
        public DbSet<CustomHealthEvent> CustomHealthEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var defaultDate = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<EventTypeDefinition>().HasData(
                new EventTypeDefinition { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Visita Veterinaria", Icon = "local_hospital", Color = "blue", CreatedAt = defaultDate },
                new EventTypeDefinition { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Medicación", Icon = "medication", Color = "orange", CreatedAt = defaultDate },
                new EventTypeDefinition { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Vacunación", Icon = "vaccines", Color = "red", CreatedAt = defaultDate }
            );

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<HealthEvent>()
                .HasDiscriminator<string>("HealthEventType")
                .HasValue<VetVisitEvent>("VetVisit")
                .HasValue<MedicationEvent>("Medication")
                .HasValue<VaccineEvent>("Vaccine")
                .HasValue<CustomHealthEvent>("Custom");

            modelBuilder.Entity<HealthEvent>()
                .HasOne(e => e.Pet)
                .WithMany(p => p.HealthEvents)
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HealthEvent>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.HealthEvents)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
