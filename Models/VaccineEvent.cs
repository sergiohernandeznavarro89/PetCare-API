namespace PetCare.API.Models
{
    public class VaccineEvent : HealthEvent
    {
        public string VaccineName { get; set; } = string.Empty;
        public int FrequencyValue { get; set; }
        public FrequencyUnit FrequencyUnit { get; set; }
    }
}
