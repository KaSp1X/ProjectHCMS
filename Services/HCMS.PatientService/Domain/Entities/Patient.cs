namespace HCMS.PatientService.Domain.Entities
{
    public class Patient
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
