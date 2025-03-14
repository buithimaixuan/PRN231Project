using BookingService.Models;

namespace BookingService.Repositories.RatingServiceRepo
{
    public interface IRatingServiceRepository
    {
        Task<ServiceRating> AddRating(ServiceRating item);
        Task<IEnumerable<ServiceRating>> GetAllRating();
        Task<ServiceRating> GetRatingById(int id);
        Task<ServiceRating> UpdateRating(ServiceRating item);
    }
}
