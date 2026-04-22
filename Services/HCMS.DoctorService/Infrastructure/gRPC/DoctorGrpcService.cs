using DoctorServiceGrpc;
using Grpc.Core;
using HCMS.DoctorService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.DoctorService.Infrastructure.gRPC
{
    public class DoctorGrpcService(ServiceDbContext context) : DoctorAvailability.DoctorAvailabilityBase
    {
        private readonly ServiceDbContext _context = context;

        public override async Task<AvailabilityResponse> CheckAvailability(
            AvailabilityRequest request,
            ServerCallContext context)
        {
            var doctorId = Guid.Parse(request.DoctorId);
            var start = DateTime.Parse(request.StartTime);
            var end = DateTime.Parse(request.EndTime);

            var hasSlot = await _context.AvailabilitySlots.AnyAsync(s =>
                s.DoctorId == doctorId &&
                start >= s.StartTime &&
                end <= s.EndTime);

            if (!hasSlot)
                return new AvailabilityResponse { IsAvailable = false };

            return new AvailabilityResponse { IsAvailable = true };
        }
    }
}
