namespace HCMS.DoctorService.Infrastructure.Kafka.Events
{
    public record AppointmentCanceledEvent(Guid AppointmentId);
}
