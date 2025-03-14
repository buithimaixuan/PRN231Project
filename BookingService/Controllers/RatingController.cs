using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IServicingService _servicingService;

        public RatingController(IServicingService servicingService)
        {
            _servicingService = servicingService;
        }

        // Lấy dánh sách tất cả service
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceRating>>> GetRatingServices()
        {
            var ratingServices = await _servicingService.GetAllRating();
            return Ok(ratingServices);
        }

        // 🔹 Lấy service theo ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceRating>> GetRatingById(int id)
        {
            var rating = await _servicingService.GetRatingById(id);
            if (rating == null)
            {
                return NotFound();
            }
            return Ok(rating);
        }

        // 🔹 Thêm service mới
        [HttpPost]
        public async Task<IActionResult> CreateRating([FromBody] RatingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data");
            }

            var newRating = new ServiceRating
            {
                ServiceId = request.ServiceId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                RatedAt = DateTime.Now
            };

            await _servicingService.AddRating(newRating);

            return CreatedAtAction(nameof(GetRatingById), new { id = newRating.RatingId }, newRating);
        }

        // 🔹 Cập nhật voucher
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingRequest request)
        {
            var checkRating = await _servicingService.GetRatingById(id);
            if (checkRating == null)
            {
                return BadRequest();
            }

            var updateRating = new ServiceRating
            {
                RatingId = checkRating.RatingId,
                ServiceId = checkRating.ServiceId,
                UserId = checkRating.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                RatedAt = checkRating.RatedAt
            };

            await _servicingService.UpdateRating(updateRating);
            return NoContent();
        }
    }
}
