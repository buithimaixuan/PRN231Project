using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using PostService.DTOs;
using PostService.Models;
using PostService.Services.Interface;

namespace PostService.Controllers
{
    [Route("api/post")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("accountWithMostPosts")]
        public async Task<IActionResult> GetTopAccountThisMonth()
        {
            var result = await _postService.GetAccountWithMostPostsThisMonth();
            return Ok(new { message = result });
        }

        [HttpGet("countPostInYear/{year}")]
        public async Task<IActionResult> GetPostCountByYear(int year)
        {
            var result = await _postService.GetPostCountByYear(year);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPosts()
        {
            List<PostDTO> res = await _postService.GetListPostAndImage();
            if (res == null)
            {
                return BadRequest("List post is null");
            }
            return Ok(res);
        }

        [HttpGet("all/available")]
        public async Task<IActionResult> GetListPostAvailable()
        {
            var res = await _postService.GetListPostAvailable() ?? new List<PostDTO>(); // Trả về danh sách rỗng nếu null
            return Ok(res);
        }

        [HttpGet("{postId}")]
        public async Task<IActionResult> GetOnePost(int postId)
        {
            if (postId < 0) return NotFound("Post Id is not valid");
            var postDto = await _postService.GetPostAndImage(postId);

            if (postDto == null) return BadRequest("Error message: Fail to get post!");
            return Ok(postDto);
        }

        [HttpGet("all/by-account")]
        public async Task<IActionResult> GetListPostByAccId([FromQuery] int accountId)
        {
            if (accountId < 0) return NotFound("Account Id is not valid");
            var postDto = await _postService.GetAllPostByAccountId(accountId);

            if (postDto == null)
                return BadRequest("Error message: Fail to get post!");
            return Ok(postDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePost([FromBody] PostRequestDTO request)
        {
            if (request == null) return BadRequest("Error message: Fail to get post!");

            var newPost = await _postService.AddPost(request.CategoryPostId, (int) request.AccountId, request.PostContent);

            if (newPost == null) return BadRequest("Error message: Fail to create post!");

            return Ok(newPost);
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost([FromBody] PostRequestDTO request, [FromRoute] int postId)
        {
            if (postId <= 0) return BadRequest("Error message: Fail to get Post Id!");

            if (request == null) return BadRequest("Error message: Fail to get Post!");

            if (postId != request.PostId) return NotFound("Error message: Post Id is not valid with request body!");

            var newPost = await _postService.UpdatePost(postId, request.CategoryPostId, (int) request.AccountId, request.PostContent);

            if (newPost <= 0) return BadRequest("Error message: Fail to update post!");

            return Ok(newPost);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute] int postId)
        {
            if (postId <= 0) return BadRequest("Error message: Fail to get Post Id!");

            int isSuccess = await _postService.DeleteAllByPostId(postId);

            return (isSuccess > 0) ? Ok(isSuccess) : BadRequest("Output message: Fail to delete post.");
        }

        [HttpPost("comment-on-post")]
        public async Task<IActionResult> CommentOnPost([FromBody] Comment comment)
        {
            try
            {
                var cmt = await _postService.AddComment(comment.AccountId, comment.PostId, comment.Content);
                return Ok(cmt);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("comments/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(int postId)
        {
            try
            {
                var comments = await _postService.GetAllCommentPostByPostId(postId);
                var commentCount = comments.Count();
                return Ok(new
                {
                    CommentCount = commentCount,
                    Comments = comments
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("like")]
        public async Task<IActionResult> AddLike([FromQuery] int accountId, [FromQuery] int postId)
        {
            try
            {
                await _postService.AddLike(accountId, postId);
                return Ok(new { Message = "Like successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Remove Like
        [HttpDelete("unlike")]
        public async Task<IActionResult> RemoveLike([FromQuery] int accountId, [FromQuery] int postId)
        {
            try
            {
                await _postService.RemoveLike(accountId, postId);
                return Ok(new { Message = "Like removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Tất cả like trong 1 bài post
        [HttpGet("all-likes/{postId}")]
        public async Task<IActionResult> GetAllLikePostByPostId(int postId)
        {
            try
            {
                // Lấy danh sách lượt thích
                var likes = await _postService.GetAllLikePostByPostId(postId);

                // Đếm số lượng lượt thích (loại trừ UnLike)
                var likeCount = likes.Count();

                return Ok(new
                {
                    LikeCount = likeCount,
                    Likes = likes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("total-post")]
        public async Task<IActionResult> GetTotalExperts()
        {
            int totalExperts = await _postService.GetTotalPostService();
            return Ok(totalExperts);
        }

    }
}
