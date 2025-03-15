using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.Models;
using PostService.Services.Implement;
using PostService.Services.Interface;

namespace PostService.Controllers
{
    [Route("api/post-categories")]
    [ApiController]
    public class PostCategories : ControllerBase
    {
        private readonly ICategoryPostService _service;

        public PostCategories(ICategoryPostService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllCategory();
            if (response == null) return BadRequest("Error message: list category is null");

            if (response.Count() <= 0) BadRequest("Error message: list category is empty");

            return Ok(response);
        }

        [HttpGet("{categoryPostId}")]
        public async Task<IActionResult> GetOne([FromRoute] int categoryPostId)
        {
            if (categoryPostId <= 0) return BadRequest("Error message: Category Id is not valid.");

            var response = await _service.FindById(categoryPostId);

            return (response != null) ? Ok(response) : NotFound("Output message: Cannot find category");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryPost request)
        {
            if (request == null) return BadRequest("Error message: Request body is invalid");

            var response = await _service.AddCatPost(request);

            return (response > 0) ? Ok("Output message: Create new post successfully") : BadRequest("Output message: Create category post fail");
        }

        [HttpPut("{categoryPostId}")]
        public async Task<IActionResult> Update([FromRoute] int categoryPostId, [FromBody] CategoryPost request)
        {
            if (categoryPostId <= 0) return BadRequest("Error message: Fail to get category post Id!");

            if (request == null) return BadRequest("Error message: Fail to get Category post!");

            if (categoryPostId != request.CategoryPostId) return NotFound("Error message: Category Post Id is not valid with request body!");

            var response = await _service.UpdateCatPost(request);

            if (response <= 0) return BadRequest("Output message: Fail to update category post!");

            return Ok("Output message: Update successfully");
        }

        [HttpDelete("{categoryPostId}")] 
        public async Task<IActionResult> Delete([FromRoute] int categoryPostId)
        {
            if (categoryPostId <= 0) return BadRequest("Error message: Fail to get Categort Post Id!");

            int isSuccess = await _service.DeleteCatPost(categoryPostId);

            return (isSuccess > 0) ? Ok("Output message: Delete successfully") : BadRequest("Output message: Fail to delete category post.");
        }
    }
}
