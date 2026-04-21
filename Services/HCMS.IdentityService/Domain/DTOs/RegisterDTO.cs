using HCMS.IdentityService.Domain.Enums;

namespace HCMS.IdentityService.Domain.DTOs
{
    public record RegisterDTO(string Email, string Password, UserRole Role);
}
