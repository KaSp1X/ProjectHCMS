namespace HCMS.AppointmentService.Domain.Queries
{
    public record GetAppointmentsQuery(Guid? PatientId, Guid? DoctorId);
}
