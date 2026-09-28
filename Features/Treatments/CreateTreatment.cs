using MediatR;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;

namespace PetCare.API.Features.Treatments
{
    public class CreateTreatmentCommand : IRequest<TreatmentDto>
    {
        public CreateTreatmentDto Dto { get; set; } = null!;
    }

    public class CreateTreatmentHandler : IRequestHandler<CreateTreatmentCommand, TreatmentDto>
    {
        private readonly AppDbContext _context;

        public CreateTreatmentHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TreatmentDto> Handle(CreateTreatmentCommand request, CancellationToken cancellationToken)
        {
            var treatment = new Treatment
            {
                Name = request.Dto.Name,
                Description = request.Dto.Description
            };

            _context.Treatments.Add(treatment);
            await _context.SaveChangesAsync(cancellationToken);

            return new TreatmentDto
            {
                Id = treatment.Id,
                Name = treatment.Name,
                Description = treatment.Description
            };
        }
    }
}
