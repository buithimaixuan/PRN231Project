namespace BookingService.Repositories.BookingServiceRepo
{
    public interface IBookingServiceRepository
    {
        Task<BookingService.Models.BookingService> GetBookingById(int id);
        Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item);
    }
}
