using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Client.ViewModel
{
    public class RegisterExpertViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [DefaultValue("Male")]
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        [Required]
        public DateOnly DateOfBirth { get; set; }
        [Required]
        [RegularExpression(@"^\+?[0-9]{7,15}$", ErrorMessage = "Số điện thoại không đúng định dạng.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(5, ErrorMessage = "Mật khẩu phải dài hơn 5 ký tự.")]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Major { get; set; }
        [Required]
        public int YearOfExperience { get; set; }
        [Required]
        public string ShortBio { get; set; }
        [Required]
        public IFormFile fileDegree {  get; set; }
        [Required]
        public IFormFile fileAvatar { get; set; }
        [Required]
        public IFormFile fileEducation { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now); // Lấy ngày hiện tại (không tính giờ)

            if (DateOfBirth >= currentDate) // So sánh với ngày hiện tại
            {
                yield return new ValidationResult($"Ngày sinh phải nhỏ hơn {currentDate}.", new[] { nameof(DateOfBirth) });
            }
        }
    }
}
