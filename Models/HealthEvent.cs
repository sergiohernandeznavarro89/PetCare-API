using System;

namespace PetCare.API.Models
{
    public abstract class HealthEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PetId { get; set; }
        public Pet Pet { get; set; } = null!;
        public Guid EventTypeId { get; set; }
        public EventTypeDefinition EventType { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal? Weight { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public Guid? ParentVisitId { get; set; }
        public VetVisitEvent? ParentVisit { get; set; }
    }
}
