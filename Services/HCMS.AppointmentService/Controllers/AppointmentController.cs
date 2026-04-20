using HCMS.AppointmentService.Domain.Commands;
using HCMS.AppointmentService.Domain.DTOs;
using HCMS.AppointmentService.Domain.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace HCMS.AppointmentService.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto, [FromServices] CreateAppointmentHandler handler)
        {
            try
            {
                var command = new CreateAppointmentCommand(
                    dto.PatientId,
                    dto.DoctorId,
                    dto.StartTime,
                    dto.EndTime);

                var result = await handler.Handle(command);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("doctor/{doctorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByDoctor(Guid doctorId, [FromServices] GetAppointmentsByDoctorHandler handler)
        {
            var appointments = await handler.Handle(doctorId);
            return Ok(appointments);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromServices] GetAllAppointmentsHandler handler)
        {
            var appointments = await handler.Handle();
            return Ok(appointments);
        }

        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(Guid id, [FromServices] CancelAppointmentHandler handler)
        {
            try
            {
                await handler.Handle(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
