namespace HCMS.DoctorService.Domain.DTOs
{
    public record CreateSlotDto(Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
