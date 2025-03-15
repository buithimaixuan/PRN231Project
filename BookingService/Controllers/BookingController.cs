using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IServicingService _servicingService;

        public BookingController(IServicingService servicingService)
        {
            _servicingService = servicingService;
        }

        // Lấy dánh sách tất cả booking
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<BookingService.Models.BookingService>>> GetBookingServices()
        {
            var bookingServices = await _servicingService.GetAllBooking();
            return Ok(bookingServices);
        }

        // Lấy dánh sách tất cả booking theo accId
        [HttpGet("all-by-accId/{id}")]
        public async Task<ActionResult<IEnumerable<BookingService.Models.BookingService>>> GetBookingServicesByAccId(int id)
        {
            var bookingServices = await _servicingService.GetAllBookingByAccId(id);
            return Ok(bookingServices);
        }

        // Lấy dánh sách tất cả booking theo serId
        [HttpGet("all-by-serId/{id}")]
        public async Task<ActionResult<IEnumerable<BookingService.Models.BookingService>>> GetBookingServicesBySerId(int id)
        {
            var bookingServices = await _servicingService.GetAllBookingBySerId(id);
            return Ok(bookingServices);
        }

        // 🔹 Lấy booking theo ID
        [HttpGet("get-by-id/{id}")]

        public async Task<ActionResult<BookingService.Models.BookingService>> GetBookingById(int id)
        {
            var booking = await _servicingService.GetBookingById(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        // 🔹 Thêm booking mới
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data");
            }

            var newBooking = new BookingService.Models.BookingService
            {
                ServiceId = request.ServiceId,
                BookingBy = request.BookingBy,
                BookingAt = DateTime.Now,
                BookingStatus = "sending",
                IsDeletedFarmer = false,
                Content = request.Content,
                IsDeletedExpert = false
            };

            await _servicingService.AddBooking(newBooking);

            return CreatedAtAction(nameof(GetBookingById), new { id = newBooking.BookingId }, newBooking);
        }

        // 🔹 Cập nhật booking
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingRequest request)
        {
            var checkBooking = await _servicingService.GetBookingById(id);
            if (checkBooking == null)
            {
                return BadRequest();
            }

            var updateBooking = new BookingService.Models.BookingService
            {
                BookingId = checkBooking.BookingId,
                ServiceId = checkBooking.ServiceId,
                BookingBy = checkBooking.BookingBy,
                BookingAt = checkBooking.BookingAt,
                BookingStatus = request.BookingStatus,
                IsDeletedFarmer = false,
                Content = checkBooking.Content,
                IsDeletedExpert = false
            };

            await _servicingService.UpdateStatusBooking(updateBooking);
            return NoContent();
        }

        // Xóa mềm booking phía farmer
        [HttpPut("soft-delete-farmer/{id}")]
        public async Task<IActionResult> SoftDeleteBookingByFarmer(int id)
        {
            var checkBooking = await _servicingService.GetBookingById(id);
            if (checkBooking == null)
            {
                return BadRequest();
            }

            checkBooking.IsDeletedFarmer = true;

            await _servicingService.UpdateStatusBooking(checkBooking);
            return NoContent();
        }

        // Xóa mềm booking phía expert
        [HttpPut("soft-delete-expert/{id}")]
        public async Task<IActionResult> SoftDeleteBookingByExpert(int id)
        {
            var checkBooking = await _servicingService.GetBookingById(id);
            if (checkBooking == null)
            {
                return BadRequest();
            }

            checkBooking.IsDeletedExpert = true;

            await _servicingService.UpdateStatusBooking(checkBooking);
            return NoContent();
        }

        // 🔹 Xóa voucher
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _servicingService.DeleteService(id);
            return NoContent();
        }
    }
}
