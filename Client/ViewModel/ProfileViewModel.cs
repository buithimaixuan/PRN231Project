using Client.DTOs;

namespace Client.ViewModel
{
    public class ProfileViewModel
    {
        public AccountDTO Account { get; set; }
        public ChangePasswordDTO ChangePassword { get; set; }
        public SetPasswordDTO SetPassword { get; set; }
    }
}
