using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.DTOs;
using PetCare.API.Features.PetTreatments;
using System.Security.Claims;

namespace PetCare.API.Controllers
{
    /// <summary>
    /// Controlador responsable de la gestión y asignación de tratamientos a mascotas específicas.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PetTreatmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PetTreatmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out Guid userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in token.");
        }

        /// <summary>
        /// Asigna un tratamiento global a una mascota concreta, con su frecuencia y alertas.
        /// </summary>
        /// <param name="dto">Datos de la asignación del tratamiento (mascota, tratamiento, próxima fecha...).</param>
        /// <returns>La asignación creada.</returns>
        [HttpPost]
        public async Task<ActionResult<PetTreatmentDto>> AssignTreatment(CreatePetTreatmentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var petTreatment = await _mediator.Send(new CreatePetTreatmentCommand { Dto = dto, UserId = userId });
                return Ok(petTreatment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}
