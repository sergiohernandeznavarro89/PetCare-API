using MediatR;
using PetCare.API.Data;
using PetCare.API.DTOs;
using PetCare.API.Models;

namespace PetCare.API.Features.PetTreatments
{
    public class CreatePetTreatmentCommand : IRequest<PetTreatmentDto>
    {
        public CreatePetTreatmentDto Dto { get; set; } = null!;
        public Guid UserId { get; set; }
    }

    public class CreatePetTreatmentHandler : IRequestHandler<CreatePetTreatmentCommand, PetTreatmentDto>
    {
        private readonly AppDbContext _context;

        public CreatePetTreatmentHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PetTreatmentDto> Handle(CreatePetTreatmentCommand request, CancellationToken cancellationToken)
        {
            var pet = await _context.Pets.FindAsync(new object[] { request.Dto.PetId }, cancellationToken);
            if (pet == null || pet.UserId != request.UserId)
            {
                throw new UnauthorizedAccessException("You do not own this pet.");
            }

            var petTreatment = new PetTreatment
            {
                PetId = request.Dto.PetId,
                TreatmentId = request.Dto.TreatmentId,
                FrequencyInDays = request.Dto.FrequencyInDays,
                NextDueDate = request.Dto.NextDueDate,
                Notify = request.Dto.Notify,
                IsActive = true
            };

            _context.PetTreatments.Add(petTreatment);
            await _context.SaveChangesAsync(cancellationToken);

            return new PetTreatmentDto
            {
                Id = petTreatment.Id,
                PetId = petTreatment.PetId,
                TreatmentId = petTreatment.TreatmentId,
                FrequencyInDays = petTreatment.FrequencyInDays,
                NextDueDate = petTreatment.NextDueDate,
                IsActive = petTreatment.IsActive,
                Notify = petTreatment.Notify
            };
        }
    }
}
