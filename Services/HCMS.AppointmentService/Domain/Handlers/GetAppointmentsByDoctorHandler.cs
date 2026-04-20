using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class GetAppointmentsByDoctorHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;
        public async Task<List<Appointment>> Handle(Guid doctorId)
        {
            return await _context.Appointments
            .Where(a => a.DoctorId == doctorId)
            .AsNoTracking()
            .ToListAsync();
        }
    }
}
