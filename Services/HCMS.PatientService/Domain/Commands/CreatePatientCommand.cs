namespace HCMS.PatientService.Domain.Commands
{
    public record CreatePatientCommand(Guid UserId, string FirstName, string LastName, DateTime DateOfBirth, string Phone);
}
