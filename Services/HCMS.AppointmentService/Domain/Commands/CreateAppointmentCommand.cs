namespace HCMS.AppointmentService.Domain.Commands
{
    public record CreateAppointmentCommand(Guid PatientId, Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
