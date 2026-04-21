namespace HCMS.AppointmentService.Infrastructure.Kafka.Events
{
    public record AppointmentCreatedEvent(Guid AppointmentId, Guid PatientId, Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
