using HCMS.DoctorService.Domain.Commands;
using HCMS.DoctorService.Domain.DTOs;
using HCMS.DoctorService.Domain.Entities;
using HCMS.DoctorService.Domain.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HCMS.DoctorService.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController :ControllerBase
    {
        [HttpPost("slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSlot(CreateSlotDto dto, [FromServices] CreateSlotHandler handler)
        {
            try
            {
                var command = new CreateSlotCommand(
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
    }
}
