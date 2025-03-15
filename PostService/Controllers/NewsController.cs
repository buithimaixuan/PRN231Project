using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostService.DAOs;
using PostService.DTOs;
using PostService.Models;
using PostService.Services.Interface;

namespace PostService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: api/news
        [HttpGet]
        public async Task<IActionResult> GetAllNews()
        {
            var newsList = await _newsService.GetAllNews();
            return Ok(newsList);
        }

        // GET: api/news/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(int id)
        {
            var news = await _newsService.GetByIdNews(id);
            if (news == null)
                return NotFound("News not found.");
            return Ok(news);
        }

        // POST: api/news
        [HttpPost]
        public async Task<IActionResult> CreateNews([FromBody] NewsDTO newsDto)
        {
            if (newsDto == null)
                return BadRequest("Invalid news data.");

            var newNews = new News
            {
                CategoryNewsId = newsDto.CategoryNewsId,
                Title = newsDto.Title,
                Content = newsDto.Content,
                ImageUrl = newsDto.ImageUrl,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                IsDeleted = false
            };

            await _newsService.AddNews(newNews);
            return CreatedAtAction(nameof(GetNewsById), new { id = newNews.NewsId }, newNews);
        }

        // PUT: api/news/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] NewsDTO newsDTO)
        {
            if (newsDTO == null)
                return BadRequest("Invalid news data.");

            var existingNews = await _newsService.GetByIdNews(id);
            if (existingNews == null)
                return NotFound("News not found.");

            existingNews.CategoryNewsId = newsDTO.CategoryNewsId;
            existingNews.Title = newsDTO.Title;
            existingNews.Content = newsDTO.Content;
            existingNews.ImageUrl = newsDTO.ImageUrl;
            existingNews.UpdatedAt = DateOnly.FromDateTime(DateTime.Now);

            await _newsService.UpdateNews(existingNews);
            return Ok(existingNews);
        }

        // DELETE: api/news/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(int id)
        {
            var existingNews = await _newsService.GetByIdNews(id);
            if (existingNews == null)
                return NotFound("News not found.");

            await _newsService.DeleteNews(id);
            return Ok("Delete News successfully!");
        }


        [HttpGet("FilterNews")]
        public async Task<IActionResult> FilterNews([FromQuery] int? categoryId, [FromQuery] string searchString = "", [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 8)
        {
            var (newsList, totalCount) = await _newsService.FilterAndPaginateNews(categoryId, searchString, pageNumber, pageSize);

            var response = new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                News = newsList
            };

            return Ok(response);
        }

    }
}
