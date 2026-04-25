using System.Security.Claims;

namespace HCMS.AppointmentService.Infrastructure.Auth
{
    public class UserContext
    {
        public Guid UserId { get; }
        public string Role { get; }

        public UserContext(ClaimsPrincipal user)
        {
            var sub = user.FindFirst("sub")?.Value ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            UserId = Guid.Parse(sub);
            Role = user.FindFirst("role")?.Value ?? user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public bool IsPatient => Role == "Patient";
        public bool IsDoctor => Role == "Doctor";
        public bool IsAdmin => Role == "Admin";
    }
}
