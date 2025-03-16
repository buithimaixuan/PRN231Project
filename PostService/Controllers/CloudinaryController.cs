using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.Config;
using PostService.DTOs;

namespace PostService.Controllers
{
    [Route("api/cloudinary")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly CloudinaryConfig _service;

        public CloudinaryController(CloudinaryConfig service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] UploadCloudDTO request)
        {
            if (request.formFile == null || request.formFile.Length == 0)
                return BadRequest("Error message: File is invalid.");

            var result = await _service.UploadImageAsync(request.formFile);
            var response = new ImageUploadResponseDTO(result.ImageUrl, result.PublicId);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteImage([FromRoute] string publicId)
        {
            var isDeleted = await _service.DeleteImageAsync(publicId);
            if (isDeleted)
            {
                return Ok("Output message: Delete successfully.");
            }
            return BadRequest("Output message: Delete fail.");
        }
    }
}
