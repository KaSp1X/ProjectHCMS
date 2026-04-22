using HCMS.PatientService.Domain.Commands;
using HCMS.PatientService.Domain.DTOs;
using HCMS.PatientService.Domain.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HCMS.PatientService.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreatePatientDto dto, [FromServices] CreatePatientHandler handler)
        {
            try
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value);

                var command = new CreatePatientCommand(userId, dto.FirstName, dto.LastName, dto.DateOfBirth,dto.Phone);

                var result = await handler.Handle(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
