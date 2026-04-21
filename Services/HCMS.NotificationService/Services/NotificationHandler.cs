using HCMS.NotificationService.Events;

namespace HCMS.NotificationService.Services
{
    public class NotificationHandler
    {
        public Task Handle(AppointmentCreatedEvent message)
        {
            Console.WriteLine($"[Notification] Appointment created:");
            Console.WriteLine($"Patient: {message.PatientId}");
            Console.WriteLine($"Doctor: {message.DoctorId}");
            Console.WriteLine($"Time: {message.StartTime} - {message.EndTime}");

            return Task.CompletedTask;
        }
    }
}
