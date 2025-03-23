using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class AccountDTO
    {
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public DateOnly? DateOfBirth { get; set; }
        [Required]
        public string? ShortBio { get; set; }
        [Required]
        public string? EducationUrl { get; set; }
        [Required]
        public int? YearOfExperience { get; set; }
        [Required]
        public string? DegreeUrl { get; set; }
        [Required]
        public string? Avatar { get; set; }
        [Required]
        public string? Major { get; set; }
        [Required]
        public string? Address { get; set; }

        public int? Otp { get; set; }
    }
}
