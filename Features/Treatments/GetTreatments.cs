using MediatR;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.API.DTOs;

namespace PetCare.API.Features.Treatments
{
    public class GetTreatmentsQuery : IRequest<IEnumerable<TreatmentDto>>
    {
    }

    public class GetTreatmentsHandler : IRequestHandler<GetTreatmentsQuery, IEnumerable<TreatmentDto>>
    {
        private readonly AppDbContext _context;

        public GetTreatmentsHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TreatmentDto>> Handle(GetTreatmentsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Treatments
                .Select(t => new TreatmentDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description
                })
                .ToListAsync(cancellationToken);
        }
    }
}
