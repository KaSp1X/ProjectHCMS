using HCMS.AppointmentService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Infrastructure.Core
{
    public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : DbContext(options)
    {
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                // Id is primary key
                entity.HasKey(x => x.Id);

                // Enum Status converts to int value
                entity.Property(x => x.Status)
                      .HasConversion<int>();

                // Index for optimised search purposes
                entity.HasIndex(x => new { x.DoctorId, x.StartTime, x.EndTime });
            });
        }
    }
}
