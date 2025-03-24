using Client.DTOs;
using Client.Models;

namespace Client.ViewModel
{
    public class PostDetailViewModel
    {
        public PostDTO PostDTO { get; set; }
        public List<Comment> ListComment { get; set; } = new List<Comment>();
        public Dictionary<int, Account> CommentAccounts { get; set; } = new Dictionary<int, Account>();
        public int CountCommentPost { get; set; }
        public int CountLikePost { get; set; }
        public int CountSharePost { get; set; }
        public bool IsLikedByUser { get; set; }
        public int View { get; set; }
        public string CommentContent { get; set; }
    }
}
