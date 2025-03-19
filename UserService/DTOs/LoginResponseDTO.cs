namespace UserService.DTOs
{
    public class LoginResponseDTO
    {
        public string Message { get; set; }
        public int AccountId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string Token { get; set; }

        public LoginResponseDTO(string message, int accountId, string username, string email, string phone, int roleId, string token)
        {
            Message = message;
            AccountId = accountId;
            Username = username;
            Email = email;
            Phone = phone;
            RoleId = roleId;
            Token = token;
        }

        public LoginResponseDTO()
        {
        }
    }
}
