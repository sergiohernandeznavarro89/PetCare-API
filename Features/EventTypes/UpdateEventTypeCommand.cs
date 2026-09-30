using MediatR;
using PetCare.API.Data;
using PetCare.API.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.EventTypes
{
    public class UpdateEventTypeCommand : IRequest<EventTypeDefinitionDto?>
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Color { get; set; }
    }

    public class UpdateEventTypeCommandHandler : IRequestHandler<UpdateEventTypeCommand, EventTypeDefinitionDto?>
    {
        private readonly AppDbContext _context;
        public UpdateEventTypeCommandHandler(AppDbContext context) => _context = context;

        public async Task<EventTypeDefinitionDto?> Handle(UpdateEventTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.EventTypeDefinitions.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null || entity.UserId != request.UserId.ToString()) return null;

            entity.Name = request.Name;
            entity.Icon = request.Icon;
            entity.Color = request.Color;

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