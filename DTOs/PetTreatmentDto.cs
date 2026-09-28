namespace PetCare.API.DTOs
{
    public class PetTreatmentDto
    {
        public Guid Id { get; set; }
        public Guid PetId { get; set; }
        public Guid TreatmentId { get; set; }
        public string TreatmentName { get; set; } = string.Empty;
        public int FrequencyInDays { get; set; }
        public DateTime NextDueDate { get; set; }
        public bool IsActive { get; set; }
        public bool Notify { get; set; }
    }

    public class CreatePetTreatmentDto
    {
        public Guid PetId { get; set; }
        public Guid TreatmentId { get; set; }
        public int FrequencyInDays { get; set; }
        public DateTime NextDueDate { get; set; }
        public bool Notify { get; set; } = true;
    }
}
