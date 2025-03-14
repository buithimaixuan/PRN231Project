using BookingService.Models;
using Microsoft.EntityFrameworkCore;
using RequestService.DAOs;

namespace BookingService.DAOs
{
    public class RatingServiceDAO : SingletonBase<RatingServiceDAO>
    {
        // Lấy danh sách tất cả đánh giá
        public async Task<IEnumerable<ServiceRating>> GetAllRating()
        {
            return await _context.ServiceRatings.ToListAsync();
        }

        // Lấy rating theo Id
        public async Task<ServiceRating> GetRatingById(int id)
        {
            var item = await _context.ServiceRatings.FirstOrDefaultAsync(c => c.RatingId == id);
            if (item == null) return null;
            return item;
        }

        // Tạo đánh giá
        public async Task<ServiceRating> AddRating(ServiceRating item)
        {
            _context.ServiceRatings.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        // Chỉnh sửa dịch vụ
        public async Task<ServiceRating> UpdateRating(ServiceRating item)
        {
            var existingItem = await GetRatingById(item.RatingId);
            if (existingItem == null) return null;

            _context.Entry(existingItem).CurrentValues.SetValues(item);
            await _context.SaveChangesAsync();
            return item;

        }
    }
}
