namespace HCMS.DoctorService.Domain.Entities
{
    public class AvailabilitySlot
    {
        public Guid Id { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
