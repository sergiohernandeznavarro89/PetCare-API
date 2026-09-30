using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.DTOs;
using PetCare.API.Features.HealthEvents;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetCare.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/pets/{petId}/health-events")]
    public class HealthEventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HealthEventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("agenda")]
        public async Task<ActionResult<IEnumerable<HealthEventDto>>> GetAgenda(Guid petId, [FromQuery] int skip = 0, [FromQuery] int take = 20)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var actualPetId = petId == Guid.Empty ? (Guid?)null : petId;
            var result = await _mediator.Send(new GetHealthAgendaQuery { UserId = userId, PetId = actualPetId, Skip = skip, Take = take });
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<HealthEventDto>>> GetHistory(Guid petId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new GetHealthHistoryQuery { UserId = userId, PetId = petId });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<HealthEventDto>> Create(Guid petId, [FromBody] HealthEventDto eventDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            eventDto.PetId = petId;
            var result = await _mediator.Send(new CreateHealthEventCommand { UserId = userId, EventDto = eventDto });
            return Created($"/api/pets/{petId}/health-events/{result.Id}", result);
        }
        [HttpPut("{eventId}")]
        public async Task<IActionResult> Update(Guid petId, Guid eventId, [FromBody] HealthEventDto eventDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            eventDto.PetId = petId;
            var result = await _mediator.Send(new UpdateHealthEventCommand { UserId = userId, EventId = eventId, EventDto = eventDto });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(Guid petId, Guid eventId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new DeleteHealthEventCommand { UserId = userId, EventId = eventId });
            if (!result) return NotFound();
            return NoContent();
        }
    }
}