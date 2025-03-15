namespace UserService.DTOs
{
    public class LoginDTO
    {
        public string Identifier { get; set; } = null!; // Email, Username hoặc PhoneNumber
        public string Password { get; set; } = null!;   // Password
    }
}
