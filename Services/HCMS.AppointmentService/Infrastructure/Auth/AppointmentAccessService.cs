using HCMS.AppointmentService.Domain.Entities;

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

        public bool CanComplete(UserContext user, Appointment appt) => user.IsAdmin || (user.IsDoctor && appt.DoctorId == user.UserId);
    }
}
