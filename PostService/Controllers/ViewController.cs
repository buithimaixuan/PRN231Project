using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.DAOs;
using PostService.Services.Interface;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewController : ControllerBase
    {
        private readonly IViewService _viewService;

        public ViewController(IViewService viewService)
        {
            _viewService = viewService;
        }
        [HttpGet("count/{postId}")]
        public async Task<IActionResult> GetViewCount(int postId)
        {
            var count = await _viewService.GetViewByPostId(postId);
            return Ok(new { postId, viewCount = count });

        }

        // API ghi nhận một lượt xem
        [HttpPost("add")]
        public async Task<IActionResult> AddView(int acc_id, int post_id)
        {
          await _viewService.AddRecordPost(acc_id, post_id);
            return Ok(new { message = "View recorded successfully" });
        }
    }
}
