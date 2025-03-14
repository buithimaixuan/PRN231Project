using BookingService.DTOs;
using BookingService.Models;
using BookingService.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CateServicesController : ControllerBase
    {
        private readonly IServicingService _servicingService;

        public CateServicesController(IServicingService servicingService)
        {
            _servicingService = servicingService;
        }

        // Lấy dánh sách tất cả cateService
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryService>>> GetCateServices()
        {
            var cateServices = await _servicingService.GetAllCategoryService();
            return Ok(cateServices);
        }

        // 🔹 Lấy cateService theo ID
        [HttpGet("{id}")]

        public async Task<ActionResult<CategoryService>> GetCateServiceById(int id)
        {
            var cateService = await _servicingService.GetCategoryServiceById(id);
            if (cateService == null)
            {
                return NotFound();
            }
            return Ok(cateService);
        }

        // 🔹 Thêm cateService mới
        [HttpPost]
        public async Task<ActionResult<CategoryService>> AddCateService(CateServiceRequest cateService)
        {
            if (cateService == null)
            {
                return BadRequest("Dữ liệu không hợp lệ");
            }

            var newCateService = new CategoryService
            {
                CategoryServiceName = cateService.CategoryServiceName,
                CategoryServiceDescription = cateService.CategoryServiceDescription
            };

            await _servicingService.AddCategoryService(newCateService);



            return CreatedAtAction(nameof(GetCateServiceById), new { id = newCateService.CategoryServiceId }, newCateService);
        }

        // 🔹 Cập nhật cateService
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCateService(int id, [FromBody] CateServiceRequest cateService)
        {
            var checkCateService = await _servicingService.GetCategoryServiceById(id);

            if (checkCateService == null)
            {
                return BadRequest("Invalid request data");
            }

            var updateCateService = new CategoryService
            {
                CategoryServiceId = checkCateService.CategoryServiceId,
                CategoryServiceName = cateService.CategoryServiceName,
                CategoryServiceDescription= cateService.CategoryServiceDescription
            };

            await _servicingService.UpdateCategoryService(updateCateService);
            return NoContent();
        }
    }
}
