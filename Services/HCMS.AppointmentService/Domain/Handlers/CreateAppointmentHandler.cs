using DoctorServiceGrpc;
using HCMS.AppointmentService.Domain.Commands;
using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Domain.Enums;
using HCMS.AppointmentService.Infrastructure.Core;
using HCMS.AppointmentService.Infrastructure.Kafka;
using HCMS.AppointmentService.Infrastructure.Kafka.Events;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class CreateAppointmentHandler(
        ServiceDbContext context,
        DoctorAvailability.DoctorAvailabilityClient grpcClient,
        KafkaProducer producer)
    {
        private readonly ServiceDbContext _context = context;
        private readonly DoctorAvailability.DoctorAvailabilityClient _grpcClient = grpcClient;
        private readonly KafkaProducer _producer = producer;

        public async Task<Appointment> Handle(CreateAppointmentCommand command)
        {
            if (command.StartTime >= command.EndTime)
                throw new Exception("Invalid time range");
            // TODO: UNCOMMENT AFTER GRPC CLIENT HAS WHOM TO CALL
            //
            //var grpcResponse = await _grpcClient.CheckAvailabilityAsync(
            //    new AvailabilityRequest
            //    {
            //        DoctorId = command.DoctorId.ToString(),
            //        StartTime = command.StartTime.ToString("O"),
            //        EndTime = command.EndTime.ToString("O")
            //    });

            //if (!grpcResponse.IsAvailable)
            //    throw new Exception("Doctor not available");

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

            var eventMessage = new AppointmentCreatedEvent(
                appointment.Id,
                appointment.PatientId,
                appointment.DoctorId,
                appointment.StartTime,
                appointment.EndTime);

            await _producer.ProduceAsync("appointment-created", eventMessage);

            return appointment;
        }
    }
}
