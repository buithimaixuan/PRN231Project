using BookingService.DAOs;
using Microsoft.EntityFrameworkCore;

namespace RequestService.DAOs
{
    public class BookingServiceDAO : SingletonBase<BookingServiceDAO>
    {
        // Lấy dịch vụ theo serviceId
        public async Task<BookingService.Models.BookingService> GetBookingById(int id)
        {
            var item = await _context.BookingServices.FirstOrDefaultAsync(c => c.ServiceId == id);
            if (item == null) return null;
            return item;
        }

        // Tạo yêu cầu
        public async Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item)
        {
            _context.BookingServices.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        // Cập nhật trạng thái đặt lịch
        //xuan mai
        public async Task UpdateStatusBooking(BookingService.Models.BookingService itemm)
        {
            var item = await _context.BookingServices.FirstOrDefaultAsync(c => c.BookingId == itemm.BookingId);
            if (item == null) return;
            _context.Entry(item).CurrentValues.SetValues(itemm);
            await _context.SaveChangesAsync();

        }

    }
}
