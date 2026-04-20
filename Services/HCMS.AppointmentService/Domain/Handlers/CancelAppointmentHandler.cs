using HCMS.AppointmentService.Domain.Enums;
using HCMS.AppointmentService.Infrastructure.Core;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class CancelAppointmentHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;

        public async Task Handle(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id) ?? throw new Exception("Not found");

            if (appointment.Status != AppointmentStatus.Scheduled)
                throw new Exception("Cannot cancel");

            appointment.Status = AppointmentStatus.Canceled;

            await _context.SaveChangesAsync();
        }
    }
}
