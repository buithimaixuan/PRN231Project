namespace BookingService.DTOs
{
    public class CateServiceRequest
    {
        public string CategoryServiceName { get; set; } = null!;

        public string? CategoryServiceDescription { get; set; }
    }
}
