using System;
using System.Collections.Generic;
using PetCare.API.Models;

namespace PetCare.API.Features.HealthEvents
{
    public static class HealthEventOccurrenceGenerator
    {
        public static List<HealthEventOccurrence> GenerateOccurrences(HealthEvent healthEvent, int frequencyValue, FrequencyUnit frequencyUnit, DateTime? endDateOverride = null)
        {
            var occurrences = new List<HealthEventOccurrence>();
            if (frequencyValue <= 0)
            {
                // Un evento no repetitivo también puede tener 1 ocurrencia para controlar su estado, 
                // o podemos decidir no generar nada y controlar el estado en el padre.
                // Según el diseño, lo generamos para que siempre aparezca en la Home como un elemento gestionable.
                occurrences.Add(new HealthEventOccurrence
                {
                    HealthEventId = healthEvent.Id,
                    ScheduledDate = healthEvent.Date,
                    Status = OccurrenceStatus.Pending
                });
                return occurrences;
            }

            var current = healthEvent.Date;
            // Si no hay fecha de fin, limitamos a 2 años en el futuro
            var maxDate = endDateOverride ?? healthEvent.EndDate ?? DateTime.UtcNow.AddYears(2);
            
            // Limit safety to avoid infinite loops
            int maxOccurrences = 5000;
            int count = 0;

            while (current <= maxDate && count < maxOccurrences)
            {
                occurrences.Add(new HealthEventOccurrence
                {
                    HealthEventId = healthEvent.Id,
                    ScheduledDate = current,
                    Status = OccurrenceStatus.Pending
                });

                if (frequencyUnit == FrequencyUnit.Hours)
                    current = current.AddHours(frequencyValue);
                else if (frequencyUnit == FrequencyUnit.Days)
                    current = current.AddDays(frequencyValue);
                else if (frequencyUnit == FrequencyUnit.Months)
                    current = current.AddMonths(frequencyValue);
                else if (frequencyUnit == FrequencyUnit.Years)
                    current = current.AddYears(frequencyValue);
                else if (frequencyUnit == FrequencyUnit.Weeks)
                    current = current.AddDays(frequencyValue * 7);
                
                count++;
            }

            return occurrences;
        }
    }
}