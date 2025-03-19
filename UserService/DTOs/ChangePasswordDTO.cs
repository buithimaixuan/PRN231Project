namespace UserService.DTOs
{
    public class ChangePasswordDTO
    {
        public string currentPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmNewPassword { get; set; }
    }
}
