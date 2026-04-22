namespace HCMS.DoctorService.Domain.Commands
{
    public record CreateSlotCommand(Guid DoctorId, DateTime StartTime, DateTime EndTime);
}
