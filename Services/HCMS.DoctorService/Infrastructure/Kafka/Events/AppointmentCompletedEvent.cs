namespace HCMS.DoctorService.Infrastructure.Kafka.Events
{
    public record AppointmentCompletedEvent(Guid AppointmentId);
}
