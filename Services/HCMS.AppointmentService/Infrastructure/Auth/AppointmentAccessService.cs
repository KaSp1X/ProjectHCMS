using HCMS.AppointmentService.Domain.Entities;
using HCMS.AppointmentService.Infrastructure.Auth;

namespace HCMS.AppointmentService.Infrastructure.Auth
{
    public class AppointmentAccessService
    {
        public bool CanCreate(UserContext user)
        {
            return user.IsPatient || user.IsAdmin;
        }

        public bool CanCancel(UserContext user, Appointment appt)
        {
            if (user.IsAdmin)
                return true;

            if (user.IsPatient && appt.PatientId == user.UserId)
                return true;

            if (user.IsDoctor && appt.DoctorId == user.UserId)
                return true;

            return false;
        }
    }
}
