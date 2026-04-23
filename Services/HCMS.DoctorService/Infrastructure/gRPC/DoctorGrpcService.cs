using DoctorServiceGrpc;
using Grpc.Core;
using HCMS.DoctorService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

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
            var start = DateTime.Parse(request.StartTime, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var end = DateTime.Parse(request.EndTime, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

            var hasSlot = await _context.AvailabilitySlots.AnyAsync(s =>
                s.DoctorId == doctorId &&
                start >= s.StartTime &&
                end <= s.EndTime);

            if (!hasSlot)
                return new AvailabilityResponse { IsAvailable = false };

            var hasOverlap = await _context.BookedAppointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                start < a.EndTime &&
                end > a.StartTime);

            if (hasOverlap)
                return new AvailabilityResponse { IsAvailable = false };

            return new AvailabilityResponse { IsAvailable = true };
        }
    }
}
