namespace HCMS.AppointmentService.Infrastructure.Kafka.Events
{
    public record AppointmentCanceledEvent(Guid AppointmentId);
}
