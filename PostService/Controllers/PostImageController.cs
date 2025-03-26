using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.DTOs;
using PostService.Services.Interface;

namespace PostService.Controllers
{
    [Route("api/image")]
    [ApiController]
    public class PostImageController : ControllerBase
    {
        private readonly IPostImageService _service;

        public PostImageController(IPostImageService service)
        {
            _service = service;
        }

        [HttpGet("by-post-id")]
        public async Task<IActionResult> GetAllByPostId([FromQuery] int postId)
        {
            if (postId <= 0)
            {
                return NotFound("Error message: Cannot found Post Id.");
            }

            var response = await _service.GetPostImagesByPostId(postId);
            
            return (response != null) ? Ok(response) : BadRequest("Output message: There are no image for this post.");
        }

        [HttpPost]
        public async Task<IActionResult> AddImageForPost([FromBody] PostImageRequestDTO request)
        {
            if (request == null)
            {
                return BadRequest("Error message: Post image object is invalid.");
            }

            await _service.AddPostImage(request.PostId, request.ImageUrl, request.PublicId);
            return Ok("Output message: Add image for post successfully.");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteImageById([FromQuery] int postImageId)
        {
            if (postImageId <= 0) return BadRequest("Error message: Could not found post image id");

            var response = await _service.DeletePostImage(postImageId);

            return (response <= 0) ? Ok("Output message: Delete post image successfully.") : BadRequest("Output message: Delete fail");
        }

        [HttpDelete("delete-all-of-post/{postId}")]
        public async Task<IActionResult> DeleteAllOfPost([FromRoute] int postId)
        {
            await _service.DeleteAllByPostId(postId);
            return Ok("Delete successfully");
        }
    }
}
