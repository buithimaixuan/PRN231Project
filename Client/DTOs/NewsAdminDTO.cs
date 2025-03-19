using Client.Models;
using System.ComponentModel.DataAnnotations;

namespace Client.DTOs
{
    public class NewsAdminDTO
    {
        public int NewsId { get; set; }
        public int CategoryNewsId { get; set; }
        public string? CategoryNewsName { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public DateOnly CreatedAt { get; set; }

        public DateOnly? UpdatedAt { get; set; }

        public DateOnly? DeletedAt { get; set; }

        public bool? IsDeleted { get; set; }
        public List<CategoryNews>? Categories { get; set; } = new();
    }
}
