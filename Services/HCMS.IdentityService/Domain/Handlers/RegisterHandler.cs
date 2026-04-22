using HCMS.IdentityService.Domain.Commands;
using HCMS.IdentityService.Domain.Entities;
using HCMS.IdentityService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.IdentityService.Domain.Handlers
{
    public class RegisterHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;

        public async Task Handle(RegisterCommand command)
        {
            var exists = await _context.Users.AnyAsync(x => x.Email == command.Email);

            if (exists)
                throw new Exception("User already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = command.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
                Role = command.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
