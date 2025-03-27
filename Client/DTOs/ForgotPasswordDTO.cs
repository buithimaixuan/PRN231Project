using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class ForgotPasswordDTO
    {
        [BindProperty]
        [Required(ErrorMessage = "Email cannot be blank")]
        [EmailAddress(ErrorMessage = "Incorrect email format")]
        public string ResetEmail { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Cannot be left blank")]
        public int OTP { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Cannot be left blank")]
        public string NewPassword { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Cannot be left blank")]
        [Compare(otherProperty: "NewPassword", ErrorMessage = "No duplicate password")]
        public string ConfirmPass { get; set; }
    }
}
