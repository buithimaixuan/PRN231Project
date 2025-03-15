using BookingService.DAOs;
using BookingService.Models;
using Microsoft.EntityFrameworkCore;

namespace RequestService.DAOs
{
    public class BookingServiceDAO : SingletonBase<BookingServiceDAO>
    {
        // Lấy danh sách tất cả booking
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBooking()
        {
            return await _context.BookingServices.ToListAsync();
        }

        // Lấy dịch vụ theo serviceId
        public async Task<BookingService.Models.BookingService> GetBookingById(int id)
        {
            var item = await _context.BookingServices.FirstOrDefaultAsync(c => c.BookingId == id);
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

        // Đếm tổng số booking đã được sử dụng theo serId
        public async Task<int> CountBookingConfirmBySerId(int id)
        {
            return await _context.BookingServices.Where(b => b.ServiceId == id && b.BookingStatus.Equals("confirmed")).CountAsync();
        }

        // Lấy danh sách tất cả booking theo account Id
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingByAccId(int id)
        {
            return await _context.BookingServices.Where(c => c.BookingBy == id && c.IsDeletedFarmer == false).ToListAsync();
        }

        // Lấy danh sách tất cả booking theo service Id
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingBySerId(int id)
        {
            return await _context.BookingServices.Where(c => c.ServiceId == id && c.IsDeletedFarmer == false).ToListAsync();
        }
    }
}
