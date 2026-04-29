using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Domain.Enums;
using HCMS.AppointmentService.Infrastructure.Auth;
using HCMS.AppointmentService.Infrastructure.Core;
using HCMS.AppointmentService.Infrastructure.Kafka.Events;
using System.Text.Json;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class CompleteAppointmentHandler(ServiceDbContext context, AppointmentAccessService access)
    {
        private readonly ServiceDbContext _context = context;
        private readonly AppointmentAccessService _access = access;

        public async Task Handle(Guid appointmentId, UserContext user)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId) ?? throw new Exception("Not found");
            
            if (!_access.CanComplete(user, appointment))
                throw new Exception("Forbidden");

            if (appointment.Status != AppointmentStatus.Scheduled)
                throw new Exception("Appointment is not scheduled");

            appointment.Status = AppointmentStatus.Completed;

            await _context.SaveChangesAsync();

            var eventMessage = new AppointmentCompletedEvent(appointment.Id);

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = "appointment-completed",
                Payload = JsonSerializer.Serialize(eventMessage),
                OccurredOn = DateTime.UtcNow,
                Processed = false
            };

            _context.OutboxMessages.Add(outboxMessage);
            await _context.SaveChangesAsync();
        }
    }
}
