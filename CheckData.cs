using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using Microsoft.Extensions.DependencyInjection;

namespace PetCare.API.Scripts
{
    public class CheckData
    {
        public static void Run(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var count = context.EventTypeDefinitions.Count();
            Console.WriteLine($"TOTAL EVENT TYPES: {count}");
            foreach (var e in context.EventTypeDefinitions.ToList()) {
                Console.WriteLine($"ID: {e.Id}, Name: {e.Name}, UserId: {e.UserId ?? "null"}");
            }
        }
    }
}