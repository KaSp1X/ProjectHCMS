using HCMS.NotificationService.Events;

namespace HCMS.NotificationService.Services
{
    public class NotificationHandler
    {
        public Task Handle(AppointmentCreatedEvent message)
        {
            Console.WriteLine($"[Notification] Appointment created:");
            Console.WriteLine($"Appointment ID: {message.AppointmentId}");
            Console.WriteLine($"Patient: {message.PatientId}");
            Console.WriteLine($"Doctor: {message.DoctorId}");
            Console.WriteLine($"Time: {message.StartTime} - {message.EndTime}");

            return Task.CompletedTask;
        }

        public Task Handle(AppointmentCanceledEvent message)
        {
            Console.WriteLine($"[Notification] Appointment canceled:");
            Console.WriteLine($"Appointment ID: {message.AppointmentId}");

            return Task.CompletedTask;
        }

        public Task Handle(AppointmentCompletedEvent message)
        {
            Console.WriteLine($"[Notification] Appointment completed:");
            Console.WriteLine($"Appointment ID: {message.AppointmentId}");

            return Task.CompletedTask;
        }

        public Task Handle(MedicalRecordCreatedEvent message)
        {
            Console.WriteLine($"[Notification] Medical record created:");
            Console.WriteLine($"Record ID: {message.RecordId}");
            Console.WriteLine($"Appointment ID: {message.AppointmentId}");
            Console.WriteLine($"Patient ID: {message.PatientId}");
            Console.WriteLine($"Doctor ID: {message.DoctorId}");

            return Task.CompletedTask;
        }
    }
}
