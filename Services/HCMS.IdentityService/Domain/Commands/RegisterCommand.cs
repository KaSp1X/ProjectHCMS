using HCMS.IdentityService.Domain.Enums;

namespace HCMS.IdentityService.Domain.Commands
{
    public record RegisterCommand(string Email, string Password, UserRole Role);
}
