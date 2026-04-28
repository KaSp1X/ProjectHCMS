namespace HCMS.NotificationService.Events
{
    public record MedicalRecordCreatedEvent(string RecordId, Guid AppointmentId, Guid PatientId, Guid DoctorId);
}
