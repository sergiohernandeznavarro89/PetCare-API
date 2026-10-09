using System;
using System.Text.Json.Serialization;

namespace PetCare.API.DTOs
{
    public class EventTypeDefinitionDto
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "healthEventType")]
    [JsonDerivedType(typeof(VetVisitEventDto), typeDiscriminator: "VetVisit")]
    [JsonDerivedType(typeof(MedicationEventDto), typeDiscriminator: "Medication")]
    [JsonDerivedType(typeof(VaccineEventDto), typeDiscriminator: "Vaccine")]
    [JsonDerivedType(typeof(CustomHealthEventDto), typeDiscriminator: "Custom")]
    public abstract class HealthEventDto
    {
        public Guid Id { get; set; }
        public Guid PetId { get; set; }
        public EventTypeDefinitionDto EventType { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public decimal? Weight { get; set; }
        public string? ParentId { get; set; }
        public bool HasCompletedOccurrences { get; set; }
        public DateTime? EndDate { get; set; }
        public List<HealthEventOccurrenceDto>? Occurrences { get; set; }
    }

    public class VetVisitEventDto : HealthEventDto
    {
        public string? VeterinarianName { get; set; }
        public string? ClinicName { get; set; }
        public string? Diagnosis { get; set; }
        public bool IsHospitalization { get; set; }
        public DateTime? DischargeDate { get; set; }
    }

    public class MedicationEventDto : HealthEventDto
    {
        public string DrugName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public int FrequencyValue { get; set; }
        public int FrequencyUnit { get; set; } // Map to enum integer or string
        public DateTime StartDate { get; set; }
    }

    public class VaccineEventDto : HealthEventDto
    {
        public string VaccineName { get; set; } = string.Empty;
        public int FrequencyValue { get; set; }
        public int FrequencyUnit { get; set; }
    }

    public class CustomHealthEventDto : HealthEventDto
    {
        public int FrequencyValue { get; set; }
        public int FrequencyUnit { get; set; }
    }
    public class HealthEventOccurrenceDto
    {
        public Guid Id { get; set; }
        public Guid HealthEventId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime? CompletedAt { get; set; }
        public HealthEventDto? HealthEvent { get; set; }
    }
}