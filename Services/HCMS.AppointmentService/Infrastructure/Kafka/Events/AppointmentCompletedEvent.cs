namespace HCMS.AppointmentService.Infrastructure.Kafka.Events
{
    public record AppointmentCompletedEvent(Guid AppointmentId);
}
