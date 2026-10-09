using System;
using System.Collections.Generic;

namespace PetCare.API.Models
{
    public class EventTypeDefinition
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? UserId { get; set; } 
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<HealthEvent> HealthEvents { get; set; } = new List<HealthEvent>();
    }
}
