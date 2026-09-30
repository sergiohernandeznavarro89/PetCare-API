using MediatR;
using PetCare.API.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PetCare.API.Features.EventTypes
{
    public class DeleteEventTypeCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }

    public class DeleteEventTypeCommandHandler : IRequestHandler<DeleteEventTypeCommand, bool>
    {
        private readonly AppDbContext _context;
        public DeleteEventTypeCommandHandler(AppDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteEventTypeCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.EventTypeDefinitions.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null || entity.UserId != request.UserId.ToString()) return false;

            _context.EventTypeDefinitions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}