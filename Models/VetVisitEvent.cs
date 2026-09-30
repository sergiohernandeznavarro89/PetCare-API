using System;

namespace PetCare.API.Models
{
    public class VetVisitEvent : HealthEvent
    {
        public string? VeterinarianName { get; set; }
        public string? ClinicName { get; set; }
        public string? Diagnosis { get; set; }
        public bool IsHospitalization { get; set; }
        public DateTime? DischargeDate { get; set; }
    }
}
