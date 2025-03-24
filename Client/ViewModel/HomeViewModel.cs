using Client.DTOs;
using Client.Models;

namespace Client.ViewModel
{
    public class HomeViewModel
    {
        public Account? Account { get; set; } // Thông tin tài khoản (null nếu chưa đăng nhập)
        public List<PostDTO>? PostDTOs { get; set; } // Danh sách bài viết
        public List<CategoryPost> categoryPosts { get; set; }
        public PostViewModel PostViewModel { get; set; }
    }
}
