using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        public string currentPassword { get; set; }
        [Required]
        public string newPassword { get; set; }
        [Required]
        public string confirmNewPassword { get; set; }
    }
}
