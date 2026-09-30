using MediatR;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.EventTypes
{
    public class CreateEventTypeCommand : IRequest<EventTypeDefinitionDto>
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }

    public class CreateEventTypeCommandHandler : IRequestHandler<CreateEventTypeCommand, EventTypeDefinitionDto>
    {
        private readonly AppDbContext _context;
        public CreateEventTypeCommandHandler(AppDbContext context) => _context = context;

        public async Task<EventTypeDefinitionDto> Handle(CreateEventTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = new EventTypeDefinition
            {
                UserId = request.UserId.ToString(),
                Name = request.Name,
                Icon = request.Icon,
                Color = request.Color
            };

            _context.EventTypeDefinitions.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new EventTypeDefinitionDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Name = entity.Name,
                Icon = entity.Icon,
                Color = entity.Color
            };
        }
    }
}
