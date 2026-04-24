namespace HCMS.MedicalRecordsService.Domain.DTOs
{
    public record CreateRecordDto(Guid PatientId, Guid DoctorId, Guid AppointmentId, string Notes);
}
