using PostService.Models;

namespace PostService.DTOs
{
    public class PostDTO
    {
        public Post post { get; set; }
        public IEnumerable<PostImage> postImages { get; set; }
        public IEnumerable<LikePost> likePosts { get; set; }
        public IEnumerable<Comment> comments { get; set; }
        public IEnumerable<SharePost> sharePosts { get; set; }

        public PostDTO()
        {
            
        }

        public PostDTO(Post post, IEnumerable<PostImage> postImages, IEnumerable<LikePost> likePosts, IEnumerable<Comment> comments, IEnumerable<SharePost> sharePosts)
        {
            this.post = post;
            this.postImages = postImages;
            this.likePosts = likePosts;
            this.comments = comments;
            this.sharePosts = sharePosts;
        }
    }
}
