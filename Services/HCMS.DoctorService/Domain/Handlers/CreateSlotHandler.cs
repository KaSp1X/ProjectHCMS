using HCMS.DoctorService.Domain.Commands;
using HCMS.DoctorService.Domain.Entities;
using HCMS.DoctorService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.DoctorService.Domain.Handlers
{
    public class CreateSlotHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;
        public async Task<AvailabilitySlot> Handle(CreateSlotCommand command)
        {
            var slot = new AvailabilitySlot
            {
                Id = Guid.NewGuid(),
                DoctorId = command.DoctorId,
                StartTime = command.StartTime,
                EndTime = command.EndTime
            };

            _context.AvailabilitySlots.Add(slot);
            await _context.SaveChangesAsync();

            return slot;
        }
    }
}
