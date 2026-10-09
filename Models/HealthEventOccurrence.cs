using System;

namespace PetCare.API.Models
{
    public enum OccurrenceStatus
    {
        Pending,
        Completed,
        Postponed,
        Cancelled
    }

    public class HealthEventOccurrence
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid HealthEventId { get; set; }
        public HealthEvent HealthEvent { get; set; } = null!;
        public DateTime ScheduledDate { get; set; }
        public OccurrenceStatus Status { get; set; } = OccurrenceStatus.Pending;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}