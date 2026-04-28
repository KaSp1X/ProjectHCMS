using HCMS.DoctorService.Domain.Commands;
using HCMS.DoctorService.Domain.DTOs;
using HCMS.DoctorService.Domain.Handlers;
using HCMS.DoctorService.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HCMS.DoctorService.Controllers
{
    [ApiController]
    [Route("api/doctors")]
    public class DoctorController(ServiceDbContext context) : ControllerBase
    {
        private readonly ServiceDbContext _context = context;

        [HttpPost("slots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSlot(CreateSlotDto dto, [FromServices] CreateSlotHandler handler)
        {
            try
            {
                if (dto.StartTime >= dto.EndTime)
                    return BadRequest("Invalid time range");

                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")?.Value);

                var exists = await _context.AvailabilitySlots.AnyAsync(a =>
                a.DoctorId == userId &&
                dto.StartTime < a.EndTime &&
                dto.EndTime > a.StartTime);

                if (exists)
                    return BadRequest("Time slot is intersected with existing one");

                var command = new CreateSlotCommand(
                    userId,
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
