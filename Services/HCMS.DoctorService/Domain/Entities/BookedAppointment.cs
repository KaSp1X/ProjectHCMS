namespace HCMS.DoctorService.Domain.Entities
{
    public class BookedAppointment
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
