using System;
using System.ComponentModel.DataAnnotations;

namespace PetCare.API.DTOs
{
    public class CreatePetDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Species { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Breed { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? PhotoUrl { get; set; }
    }
}
