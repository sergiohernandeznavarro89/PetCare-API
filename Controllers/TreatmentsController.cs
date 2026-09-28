using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetCare.API.DTOs;
using PetCare.API.Features.Treatments;

namespace PetCare.API.Controllers
{
    /// <summary>
    /// Controlador responsable de gestionar el catálogo global de tratamientos base (ej. Vacuna Rabia, Chip...).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TreatmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TreatmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Obtiene el catálogo completo de tratamientos.
        /// </summary>
        /// <returns>Lista de tratamientos globales.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreatmentDto>>> GetTreatments()
        {
            var treatments = await _mediator.Send(new GetTreatmentsQuery());
            return Ok(treatments);
        }

        /// <summary>
        /// Añade un nuevo tratamiento global al catálogo.
        /// </summary>
        /// <param name="dto">El nombre y descripción del tratamiento.</param>
        /// <returns>El nuevo tratamiento guardado.</returns>
        [HttpPost]
        public async Task<ActionResult<TreatmentDto>> PostTreatment(CreateTreatmentDto dto)
        {
            var treatment = await _mediator.Send(new CreateTreatmentCommand { Dto = dto });
            return Ok(treatment);
        }
    }
}
