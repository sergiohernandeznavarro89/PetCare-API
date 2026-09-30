using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.EventTypes
{
    public class GetEventTypesQuery : IRequest<IEnumerable<EventTypeDefinitionDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetEventTypesQueryHandler : IRequestHandler<GetEventTypesQuery, IEnumerable<EventTypeDefinitionDto>>
    {
        private readonly AppDbContext _context;
        public GetEventTypesQueryHandler(AppDbContext context) => _context = context;

        public async Task<IEnumerable<EventTypeDefinitionDto>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
        {
            var eventTypes = await _context.EventTypeDefinitions
                .Where(e => e.UserId == null || e.UserId == request.UserId.ToString())
                .ToListAsync(cancellationToken);

            return eventTypes.Select(e => new EventTypeDefinitionDto
            {
                Id = e.Id,
                UserId = e.UserId,
                Name = e.Name,
                Icon = e.Icon,
                Color = e.Color
            });
        }
    }
}
