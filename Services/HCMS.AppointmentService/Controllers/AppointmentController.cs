using HCMS.AppointmentService.Domain.Commands;
using HCMS.AppointmentService.Domain.DTOs;
using HCMS.AppointmentService.Domain.Handlers;
using HCMS.AppointmentService.Domain.Queries;
using HCMS.AppointmentService.Infrastructure.Auth;
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto, [FromServices] CreateAppointmentHandler handler, [FromServices] AppointmentAccessService access)
        {
            var user = new UserContext(User);

            var command = new CreateAppointmentCommand(
                user.UserId,
                dto.DoctorId,
                dto.StartTime,
                dto.EndTime);

            try
            {
                var result = await handler.Handle(command, user);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] Guid? patientId, [FromQuery] Guid? doctorId, [FromServices] GetAppointmentsHandler handler)
        {
            var appointments = await handler.Handle(new GetAppointmentsQuery(patientId, doctorId));
            return Ok(appointments);
        }

        [HttpPut("{appointmentId}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Cancel(Guid appointmentId, [FromServices] CancelAppointmentHandler handler)
        {
            var user = new UserContext(User);

            try
            {
                await handler.Handle(appointmentId, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{appointmentId}/complete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Complete(Guid appointmentId, [FromServices] CompleteAppointmentHandler handler)
        {
            var user = new UserContext(User);

            try
            {
                await handler.Handle(appointmentId, user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
