using HCMS.DoctorService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCMS.DoctorService.Infrastructure.Core
{
    public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : DbContext(options)
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<AvailabilitySlot> AvailabilitySlots { get; set; }
        public DbSet<BookedAppointment> BookedAppointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Primary key
            modelBuilder.Entity<Doctor>()
                        .HasKey(x => x.Id);

            // Indexes for optimized search
            modelBuilder.Entity<AvailabilitySlot>()
                        .HasIndex(x => new { x.DoctorId, x.StartTime, x.EndTime });
        }
    }
}
