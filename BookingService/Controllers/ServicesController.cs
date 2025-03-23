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
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Service>>> GetServices()
        {
            var services = await _servicingService.GetAllService();
            return Ok(services);
        }

        // Lấy dánh sách dịch vụ khả dụng
        [HttpGet("available")] // api/Services/available
        public async Task<ActionResult<IEnumerable<Service>>> GetServicesAvailable()
        {
            var services = await _servicingService.GetAllServiceAvailable();
            return Ok(services);
        }

        // Đếm tống số dịch vụ
        [HttpGet("count-all")] // api/Services/countAll
        public async Task<ActionResult<int>> GetTotalServicesCount()
        {
            int countAllServices = await _servicingService.GetTotalServicesCount();
            return Ok(countAllServices);
        }

        // Lấy dánh sách dịch vụ theo accountId
        [HttpGet("all-by-accId/{id}")]
        public async Task<ActionResult<int>> GetAllServiceByAccId(int id)
        {
            var servicesByAccId = await _servicingService.GetAllServiceByAccId(id);
            return Ok(servicesByAccId);
        }

        // 🔹 Lấy service theo ID
        [HttpGet("get-by-id/{id}")]

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
                IsEnable = true,
                IsDeleted = false, // Mặc định chưa bị xóa
                AverageRating = 0,
                RatingCount = 0
            };

            await _servicingService.AddService(newService);

            return CreatedAtAction(nameof(GetServiceById), new { id = newService.ServiceId }, newService);
        }

        // 🔹 Cập nhật service
        [HttpPut("update/{id}")]
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
                AverageRating = request.AverageRating,
                RatingCount = request.RatingCount
            };

            await _servicingService.UpdateService(updateService);
            return NoContent();
        }

        // 🔹 Xóa mềm service
        [HttpPut("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteService(int id)
        {
            var checkService = await _servicingService.GetServiceById(id);
            if (checkService == null)
            {
                return BadRequest();
            }

            checkService.IsDeleted = true;
            checkService.DeletedAt = DateTime.Now;

            await _servicingService.UpdateService(checkService);
            return NoContent();
        }

        // 🔹 Xóa cứng service
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            await _servicingService.DeleteService(id);
            return NoContent();
        }
    }
}
