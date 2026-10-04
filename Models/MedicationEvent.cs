using System;

namespace PetCare.API.Models
{
    public enum FrequencyUnit
    {
        Hours,
        Days,
        Months,
        Years,
        Weeks
    }

    public class MedicationEvent : HealthEvent
    {
        public string DrugName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public int FrequencyValue { get; set; }
        public FrequencyUnit FrequencyUnit { get; set; }
        public DateTime StartDate { get; set; }
    }
}
