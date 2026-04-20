namespace HCMS.AppointmentService.Domain.DTOs
{
    public record CreateAppointmentDto(Guid PatientId, Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
