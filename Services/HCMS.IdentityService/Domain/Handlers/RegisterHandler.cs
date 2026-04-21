using HCMS.IdentityService.Infrastructure.Core;

namespace HCMS.IdentityService.Domain.Handlers
{
    public class RegisterHandler(ServiceDbContext context)
    {
        private readonly ServiceDbContext _context = context;

        public async Task Handle()
        {
        }
    }
}
