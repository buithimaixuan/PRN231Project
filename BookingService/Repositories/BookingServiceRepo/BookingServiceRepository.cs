using RequestService.DAOs;

namespace BookingService.Repositories.BookingServiceRepo
{
    public class BookingServiceRepository : IBookingServiceRepository
    {
        private readonly BookingServiceDAO _bookingDAO;
        public BookingServiceRepository(BookingServiceDAO bookingDAO)
        {
            _bookingDAO = bookingDAO;
        }
        public async Task<BookingService.Models.BookingService> GetBookingById(int id)
        {
            return await _bookingDAO.GetBookingById(id);
        }
        public async Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item)
        {
            return await _bookingDAO.AddBooking(item);
        }
    }
}
