﻿namespace UserService.DTOs
{
    public class OTPUpdateDTO
    {
        public string FullName { get; set; }
        public string ShortBio { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Address { get; set; }
        public string EducationUrl { get; set; }
        public int? YearOfExperience { get; set; }
        public string DegreeUrl { get; set; }
        public string Major { get; set; }
        public int? OTP { get; set; }
    }
}
