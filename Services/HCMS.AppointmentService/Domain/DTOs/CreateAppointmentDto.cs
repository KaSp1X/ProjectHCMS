namespace HCMS.AppointmentService.Domain.DTOs
{
    public record CreateAppointmentDto(Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
