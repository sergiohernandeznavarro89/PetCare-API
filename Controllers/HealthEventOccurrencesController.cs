using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.Features.HealthEvents;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetCare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HealthEventOccurrencesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HealthEventOccurrencesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(Guid id)
        {
            var result = await _mediator.Send(new CompleteOccurrenceCommand { UserId = GetUserId(), OccurrenceId = id });
            if (!result) return NotFound();
            return Ok();
        }

        public class PostponeRequest { public DateTime NewDate { get; set; } }

        [HttpPatch("{id}/postpone")]
        public async Task<IActionResult> Postpone(Guid id, [FromBody] PostponeRequest req)
        {
            var result = await _mediator.Send(new PostponeOccurrenceCommand { UserId = GetUserId(), OccurrenceId = id, NewDate = req.NewDate });
            if (!result) return NotFound();
            return Ok();
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _mediator.Send(new CancelOccurrenceCommand { UserId = GetUserId(), OccurrenceId = id });
            if (!result) return NotFound();
            return Ok();
        }
    }
}