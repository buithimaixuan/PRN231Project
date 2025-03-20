using BookingService.Models;

namespace BookingService.DTOs
{
    public class BookingRequest
    {
        public int ServiceId { get; set; }
        public int BookingBy { get; set; }
        public string BookingStatus { get; set; } = null!;
        public string? Content { get; set; }
        public bool? IsDeletedFarmer { get; set; }
        public bool? IsDeletedExpert { get; set; }
    }
}