using HCMS.PatientService.Domain.Commands;
using HCMS.PatientService.Domain.Entities;
using HCMS.PatientService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.PatientService.Domain.Handlers
{
    public class CreatePatientHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;

        public async Task<Patient> Handle(CreatePatientCommand command)
        {
            var exists = await _context.Patients.AnyAsync(x => x.UserId == command.UserId);

            if (exists)
                throw new Exception("Patient profile already exists");

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                FirstName = command.FirstName,
                LastName = command.LastName,
                DateOfBirth = command.DateOfBirth,
                Phone = command.Phone,
                CreatedAt = DateTime.UtcNow
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return patient;
        }
    }
}
