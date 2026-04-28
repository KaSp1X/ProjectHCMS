namespace HCMS.MedicalRecordsService.Infrastructure.Kafka.Events
{
    public record MedicalRecordCreatedEvent(string RecordId, Guid AppointmentId, Guid PatientId, Guid DoctorId);
}
