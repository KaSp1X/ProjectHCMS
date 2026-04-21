namespace HCMS.NotificationService.Events
{
    public record AppointmentCreatedEvent(Guid AppointmentId, Guid PatientId, Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
