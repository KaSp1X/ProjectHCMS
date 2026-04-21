using HCMS.IdentityService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HCMS.IdentityService.Infrastructure.Core
{
    public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                // Id is primary key
                entity.HasKey(x => x.Id);

                // Enum Role converts to int value
                entity.Property(x => x.Role)
                      .HasConversion<int>();
            });
        }
    }
}
