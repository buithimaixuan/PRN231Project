using Client.Models;

namespace Client.ViewModel
{
    public class HomeViewModel
    {
        public Account? Account { get; set; } // Thông tin tài khoản (null nếu chưa đăng nhập)
        public List<Post>? Posts { get; set; } // Danh sách bài viết
    }
}
