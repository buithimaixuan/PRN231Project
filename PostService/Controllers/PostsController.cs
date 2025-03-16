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

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPosts()
        {
            List<PostDTO> res = await _postService.GetListPostAndImage();
            if (res == null)
            {
                return BadRequest("List post is null");
            }
            else if (res.Count <= 0)
            {
                return BadRequest("List post is empty");
            }
            return Ok(res);
        }

        [HttpGet("all/available")]
        public async Task<IActionResult> GetListPostAvailable()
        {
            List<PostDTO> res = await _postService.GetListPostAvailable();
            if (res == null)
            {
                return BadRequest("List post is null");
            }
            else if (res.Count <= 0)
            {
                return BadRequest("List post is empty");
            }
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetOnePost([FromQuery] int postId)
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
            else if (postDto.Count <= 0)
                return BadRequest("Error message: List is empty!");

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

            return Ok("Output message: Update post successfully");
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost([FromRoute] int postId)
        {
            if (postId <= 0) return BadRequest("Error message: Fail to get Post Id!");

            int isSuccess = await _postService.DeletePost(postId);

            return (isSuccess > 0) ? Ok(isSuccess) : BadRequest("Output message: Fail to delete post.");
        }




        [HttpGet("total-post")]
        public async Task<IActionResult> GetTotalExperts()
        {
            int totalExperts = await _postService.GetTotalPostService();
            return Ok(totalExperts);
        }






    }
}
