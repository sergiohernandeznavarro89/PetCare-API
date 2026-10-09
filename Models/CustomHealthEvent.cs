namespace PetCare.API.Models
{
    public class CustomHealthEvent : HealthEvent
    {
        public int FrequencyValue { get; set; }
        public FrequencyUnit FrequencyUnit { get; set; }
    }
}
