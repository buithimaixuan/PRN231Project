using BookingService.Models;
using RequestService.DAOs;

namespace BookingService.DAOs
{
    public class RatingServiceDAO : SingletonBase<RatingServiceDAO>
    {
        // Tạo đánh giá
        public async Task<ServiceRating> AddRating(ServiceRating item)
        {
            _context.ServiceRatings.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
