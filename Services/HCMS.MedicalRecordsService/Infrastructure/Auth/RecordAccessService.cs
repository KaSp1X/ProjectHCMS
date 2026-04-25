using HCMS.MedicalRecordsService.Domain.Entities;

namespace HCMS.MedicalRecordsService.Infrastructure.Auth
{
    public class RecordAccessService
    {
        public bool CanAccess(UserContext user, MedicalRecord record)
        {
            if (user.IsAdmin)
                return true;

            if (user.IsPatient && record.PatientId == user.UserId)
                return true;

            if (user.IsDoctor && record.DoctorId == user.UserId)
                return true;

            return false;
        }
    }
}
