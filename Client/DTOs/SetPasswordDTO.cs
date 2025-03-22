using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class SetPasswordDTO
    {
        [Required(ErrorMessage = "New password is required.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm new password is required.")]
        public string ConfirmNewPassword { get; set; }
    }
}
