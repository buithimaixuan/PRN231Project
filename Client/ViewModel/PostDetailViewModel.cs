using Client.DTOs;
using Client.Models;

namespace Client.ViewModel
{
    public class PostDetailViewModel
    {
        public PostDTO PostDTO { get; set; }
        public List<Comment> ListComment { get; set; }
        public Dictionary<int?, Account> CommentAccounts { get; set; } // Thay đổi từ int sang int?
        public int CountCommentPost { get; set; }
        public int CountLikePost { get; set; }
        public int CountSharePost { get; set; }
        public bool IsLikedByUser { get; set; }
        public int View { get; set; }
        public string CommentContent { get; set; }
        public CategoryPost CategoryPost { get; set; }
        public List<string> PostImageUrls { get; set; }

        public PostDetailViewModel()
        {
            CommentAccounts = new Dictionary<int?, Account>();
        }
    }
}
