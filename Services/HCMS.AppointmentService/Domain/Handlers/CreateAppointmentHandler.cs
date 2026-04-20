using DoctorServiceGrpc;
using HCMS.AppointmentService.Domain.Commands;
using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Domain.Enums;
using HCMS.AppointmentService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class CreateAppointmentHandler(
        ServiceDbContext context,
        DoctorAvailability.DoctorAvailabilityClient grpcClient)
    {
        private readonly ServiceDbContext _context = context;
        private readonly DoctorAvailability.DoctorAvailabilityClient _grpcClient = grpcClient;

        public async Task<Appointment> Handle(CreateAppointmentCommand command)
        {
            if (command.StartTime >= command.EndTime)
                throw new Exception("Invalid time range");

            var grpcResponse = await _grpcClient.CheckAvailabilityAsync(
                new AvailabilityRequest
                {
                    DoctorId = command.DoctorId.ToString(),
                    StartTime = command.StartTime.ToString("O"),
                    EndTime = command.EndTime.ToString("O")
                });

            if (!grpcResponse.IsAvailable)
                throw new Exception("Doctor not available");

            var exists = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == command.DoctorId &&
                a.Status == AppointmentStatus.Scheduled &&
                command.StartTime < a.EndTime &&
                command.EndTime > a.StartTime);

            if (exists)
                throw new Exception("Time slot already taken");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = command.PatientId,
                DoctorId = command.DoctorId,
                StartTime = command.StartTime,
                EndTime = command.EndTime,
                Status = AppointmentStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return appointment;
        }
    }
}
