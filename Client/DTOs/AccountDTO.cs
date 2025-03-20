namespace Client.DTOs
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string? ShortBio { get; set; }

        public string? EducationUrl { get; set; }

        public int? YearOfExperience { get; set; }

        public string? DegreeUrl { get; set; }

        public string? Avatar { get; set; }

        public string? Major { get; set; }

        public string? Address { get; set; }
    }
}
