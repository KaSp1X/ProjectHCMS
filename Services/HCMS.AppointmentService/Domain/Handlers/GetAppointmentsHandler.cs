using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Domain.Queries;
using HCMS.AppointmentService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Domain.Handlers
{
    public class GetAppointmentsHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;
        public async Task<List<Appointment>> Handle(GetAppointmentsQuery query)
        {
            var q = _context.Appointments.AsNoTracking().AsQueryable();

            if (query.PatientId.HasValue)
                q = q.Where(x => x.PatientId == query.PatientId);

            if (query.DoctorId.HasValue)
                q = q.Where(x => x.DoctorId == query.DoctorId);

            return await q.ToListAsync();
        }
    }
}
