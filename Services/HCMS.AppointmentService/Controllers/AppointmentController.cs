using DoctorServiceGrpc;
using HCMS.AppointmentService.Domain.DTOs;
using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Domain.Enums;
using HCMS.AppointmentService.Infrastructure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentController(ServiceDbContext context, DoctorAvailability.DoctorAvailabilityClient grpcClient) : ControllerBase
    {
        private readonly ServiceDbContext _context = context;
        private readonly DoctorAvailability.DoctorAvailabilityClient _grpcClient = grpcClient;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                return BadRequest("Invalid time range");

            // RESTfull way to check if doc is available
            //
            //var exists = await _context.Appointments.AnyAsync(a =>
            //    a.DoctorId == dto.DoctorId &&
            //    a.Status == AppointmentStatus.Scheduled &&
            //    dto.StartTime < a.EndTime &&
            //    dto.EndTime > a.StartTime);
            //
            //if (exists)
            //    return BadRequest("Doctor already has an appointment in this time slot");

            var grpcResponse = await _grpcClient.CheckAvailabilityAsync(
                new AvailabilityRequest
                {
                    DoctorId = dto.DoctorId.ToString(),
                    StartTime = dto.StartTime.ToString("O"),
                    EndTime = dto.EndTime.ToString("O")
                });

            if (!grpcResponse.IsAvailable)
                return BadRequest("Doctor not available (gRPC)");

            var appointment = new Appointment(
                Guid.NewGuid(),
                dto.PatientId,
                dto.DoctorId,
                dto.StartTime,
                dto.EndTime,
                AppointmentStatus.Scheduled,
                DateTime.UtcNow
            );

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _context.Appointments.AsNoTracking().ToListAsync();
            return Ok(appointments);
        }
    }
}
