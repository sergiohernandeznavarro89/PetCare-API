using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.API.Models
{
    [Table("PetTreatment")]
    public class PetTreatment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PetId { get; set; }
        [ForeignKey(nameof(PetId))]
        public Pet? Pet { get; set; }

        public Guid TreatmentId { get; set; }
        [ForeignKey(nameof(TreatmentId))]
        public Treatment? Treatment { get; set; }

        [Required]
        public int FrequencyInDays { get; set; }

        [Required]
        public DateTime NextDueDate { get; set; } // Map to DATE in DB

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<PetTreatmentHistory> History { get; set; } = new List<PetTreatmentHistory>();
    }
}
