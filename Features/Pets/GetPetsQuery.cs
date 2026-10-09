using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;

namespace PetCare.API.Features.Pets
{
    public class GetPetsQuery : IRequest<IEnumerable<PetDto>>
    {
        public Guid UserId { get; set; }
    }

    public class GetPetsHandler : IRequestHandler<GetPetsQuery, IEnumerable<PetDto>>
    {
        private readonly AppDbContext _context;

        public GetPetsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PetDto>> Handle(GetPetsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Pets
                .Where(p => p.UserId == request.UserId)
                .Select(p => new PetDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Species = p.Species,
                    Breed = p.Breed,
                    DateOfBirth = p.DateOfBirth,
                    PhotoUrl = p.PhotoUrl,
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
