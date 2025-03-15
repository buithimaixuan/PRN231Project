using BookingService.DAOs;
using BookingService.Models;
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
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBooking()
        {
            return await _bookingDAO.GetAllBooking();
        }
        public async Task<BookingService.Models.BookingService> GetBookingById(int id)
        {
            return await _bookingDAO.GetBookingById(id);
        }
        public async Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item)
        {
            return await _bookingDAO.AddBooking(item);
        }
        public async Task UpdateStatusBooking(BookingService.Models.BookingService item)
        {
            await _bookingDAO.UpdateStatusBooking(item);
        }
        public async Task<int> CountBookingConfirmBySerId(int id)
        {
            return await _bookingDAO.CountBookingConfirmBySerId(id);
        }
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingByAccId(int id)
        {
            return await _bookingDAO.GetAllBookingByAccId(id);
        }
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingBySerId(int id)
        {
            return await _bookingDAO.GetAllBookingBySerId(id);
        }
    }
}
