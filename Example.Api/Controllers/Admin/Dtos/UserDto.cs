namespace Example.Api.Controllers.Admin.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool EmailVerified { get; set; }
        public DateTime? EmailVerificationDate { get; set; }
        public bool IsAdmin { get; set; } = false;
    }
}
