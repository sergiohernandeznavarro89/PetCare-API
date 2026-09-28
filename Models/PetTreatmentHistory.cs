using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetCare.API.Models
{
    [Table("PetTreatmentHistory")]
    public class PetTreatmentHistory
    {
        [Key]
        public Guid Id { get; set; }

        public Guid PetTreatmentId { get; set; }
        [ForeignKey(nameof(PetTreatmentId))]
        public PetTreatment? PetTreatment { get; set; }

        [Required]
        public DateTime AppliedDate { get; set; } // Map to DATE in DB

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
