using HCMS.PatientService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCMS.PatientService.Infrastructure.Core
{
    public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : DbContext(options)
    {
        public DbSet<Patient> Patients { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>(entity =>
            {
                // Primary key
                entity.HasKey(x => x.Id);

                // Index  + unique
                entity.HasIndex(x => x.UserId).IsUnique();
            });
        }
    }
}
