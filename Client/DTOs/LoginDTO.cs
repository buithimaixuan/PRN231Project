using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Identifier { get; set; } = null!; // Email, Username hoặc PhoneNumber
        [Required]
        public string Password { get; set; } = null!;   // Password
    }
}
