using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.Features.EventTypes;
using PetCare.API.DTOs;
using System.Linq;

namespace PetCare.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventTypeDefinitionDto>>> GetEventTypes()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new GetEventTypesQuery { UserId = userId });
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EventTypeDefinitionDto>> CreateEventType([FromBody] CreateEventTypeCommand command)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            command.UserId = userId;
            var result = await _mediator.Send(command);
            return Created($"/api/EventTypes/{result.Id}", result);
        }
    }
}
