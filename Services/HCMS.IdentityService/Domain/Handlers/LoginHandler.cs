using HCMS.IdentityService.Domain.Commands;
using HCMS.IdentityService.Infrastructure.Core;
using HCMS.IdentityService.Services;
using Microsoft.EntityFrameworkCore;

namespace HCMS.IdentityService.Domain.Handlers
{
    public class LoginHandler(ServiceDbContext context, JwtService jwt)
    {
        private readonly ServiceDbContext _context = context;
        private readonly JwtService _jwt = jwt;

        public async Task<string> Handle(LoginCommand command)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == command.Email);

            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(command.Password, user.PasswordHash))
            {
                throw new Exception("User doesn't exist or wrong password");
            }

            var token = _jwt.Generate(user);

            return token;
        }
    }
}
