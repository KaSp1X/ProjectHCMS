namespace HCMS.PatientService.Domain.DTOs
{
    public record CreatePatientDto(string FirstName, string LastName, DateTime DateOfBirth, string Phone);
}
