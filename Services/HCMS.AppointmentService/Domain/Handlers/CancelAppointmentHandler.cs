using HCMS.AppointmentService.Domain.Enums;
using HCMS.AppointmentService.Infrastructure.Auth;
using HCMS.AppointmentService.Infrastructure.Core;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class CancelAppointmentHandler(ServiceDbContext context, AppointmentAccessService access)
    {
        private readonly ServiceDbContext _context = context;
        private readonly AppointmentAccessService _access = access;

        public async Task Handle(Guid appointmentId, UserContext user)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId) ?? throw new Exception("Not found");

            if (!_access.CanCancel(user, appointment))
                throw new Exception("Forbidden");

            if (appointment.Status != AppointmentStatus.Scheduled)
                throw new Exception("Appointment is not scheduled");

            if (appointment.StartTime < DateTime.UtcNow)
                throw new Exception("Cannot cancel past appointment");

            appointment.Status = AppointmentStatus.Canceled;

            await _context.SaveChangesAsync();
        }
    }
}
