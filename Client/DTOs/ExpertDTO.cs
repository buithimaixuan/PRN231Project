using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class ExpertDTO
    {
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

        public DateTime? DateOfBirth { get; set; }
        [Required]

        public string? ShortBio { get; set; }
        

        public string? EducationUrl { get; set; }
        [Required]

        public int? YearOfExperience { get; set; }

        public string? DegreeUrl { get; set; }

        public string? Avatar { get; set; }
        [Required]

        public string? Major { get; set; }
        [Required]

        public string? Address { get; set; }



    }

}
