using HCMS.AppointmentService.Domain.Enums;

namespace HCMS.AppointmentService.Domain.Entities
{
    public record Appointment(
        Guid Id,
        Guid PatientId,
        Guid DoctorId,
        DateTime StartTime,
        DateTime EndTime,
        AppointmentStatus Status,
        DateTime CreatedAt);
}
