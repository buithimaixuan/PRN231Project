namespace BookingService.DTOs
{
    public class ServiceRequest
    {
        public int CreatorId { get; set; }
        public int CategoryServiceId { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public double Price { get; set; }
        public bool IsEnable { get; set; }
        public bool? IsDeleted { get; set; }
        public decimal? AverageRating { get; set; }
        public int? RatingCount { get; set; }

    }
}
