using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class GetAllAppointmentsHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;
        public async Task<List<Appointment>> Handle()
        {
            return await _context.Appointments
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
