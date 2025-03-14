using Azure.Core;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServicingService _servicingService;

        public ServicesController(IServicingService servicingService)
        {
            _servicingService = servicingService;
        }

        // Lấy dánh sách tất cả service
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _servicingService.GetAllService();
            return Ok(services);
        }

        // 🔹 Lấy service theo ID
        [HttpGet("{id}")]

        public async Task<ActionResult<Service>> GetServiceById(int id)
        {
            var service = await _servicingService.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            return Ok(service); // Trả về vourcher dưới dạng OkResult
        }

        // 🔹 Thêm service mới
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ServiceRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid request data");
            }

            var newService = new Service
            {
                CreatorId = request.CreatorId,
                CategoryServiceId = request.CategoryServiceId,
                CreateAt = DateTime.Now,
                UpdatedAt = null,
                DeletedAt = null,
                Title = request.Title,
                Content = request.Content,
                Price = request.Price,
                IsEnable = request.IsEnable,
                IsDeleted = false, // Mặc định chưa bị xóa
                AverageRating = 0,
                RatingCount = 0
            };

            await _servicingService.AddService(newService);

            return CreatedAtAction(nameof(GetServiceById), new { id = newService.ServiceId }, newService);
        }

        // 🔹 Cập nhật voucher
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id,[FromBody] ServiceRequest request)
        {
            var checkService = await _servicingService.GetServiceById(id);
            if (checkService == null)
            {
                return BadRequest();
            }

            var updateService = new Service
            {
                ServiceId = checkService.ServiceId,
                CreatorId = request.CreatorId,
                CategoryServiceId = request.CategoryServiceId,
                CreateAt = checkService.CreateAt,
                UpdatedAt = DateTime.Now,
                DeletedAt = null,
                Title = request.Title,
                Content = request.Content,
                Price = request.Price,
                IsEnable = request.IsEnable,
                IsDeleted = false, // Mặc định chưa bị xóa
                AverageRating = 0,
                RatingCount = 0
            };

            await _servicingService.UpdateService(updateService);
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
