using BookingService.Models;

namespace BookingService.Repositories.RatingServiceRepo
{
    public interface IRatingServiceRepository
    {
        Task<ServiceRating> AddRating(ServiceRating item);
    }
}
