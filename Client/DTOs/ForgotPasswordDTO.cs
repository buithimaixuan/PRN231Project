using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class ForgotPasswordDTO
    {
        [BindProperty]
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Sai định dạng email")]
        public string ResetEmail { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Không được để trống")]
        public int OTP { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Không được để trống")]
        public string NewPassword { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Không được để trống")]
        [Compare(otherProperty: "NewPassword", ErrorMessage = "Không trùng mật khẩu")]
        public string ConfirmPass { get; set; }
    }
}
