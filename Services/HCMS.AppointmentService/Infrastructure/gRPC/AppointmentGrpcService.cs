using AppointmentServiceGrpc;
using Grpc.Core;
using HCMS.AppointmentService.Infrastructure.Core;
using Microsoft.EntityFrameworkCore;

namespace HCMS.AppointmentService.Infrastructure.gRPC
{
    public class AppointmentGrpcService(ServiceDbContext context) : AppointmentExistence.AppointmentExistenceBase
    {
        private readonly ServiceDbContext _context = context;

        public override async Task<GetAppointmentResponse> GetAppointment(
            GetAppointmentRequest request,
            ServerCallContext context)
        {
            var id = Guid.Parse(request.AppointmentId);

            var appt = await _context.Appointments.FirstOrDefaultAsync(x => x.Id == id);

            return appt == null
                ? throw new RpcException(new Status(StatusCode.NotFound, "Not found"))
                : new GetAppointmentResponse
                {
                    AppointmentId = appt.Id.ToString(),
                    DoctorId = appt.DoctorId.ToString(),
                    PatientId = appt.PatientId.ToString(),
                    Status = appt.Status.ToString()
                };
        }
    }
}
