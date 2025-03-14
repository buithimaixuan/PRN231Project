namespace BookingService.DTOs
{
    public class RatingRequest
    {
        public int ServiceId { get; set; }

        public int UserId { get; set; }

        public decimal Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime RatedAt { get; set; }
    }
}
