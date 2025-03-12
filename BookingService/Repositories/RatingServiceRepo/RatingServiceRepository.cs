using BookingService.DAOs;
using BookingService.Models;

namespace BookingService.Repositories.RatingServiceRepo
{
    public class RatingServiceRepository : IRatingServiceRepository
    {
        private readonly RatingServiceDAO _rateDAO;
        public RatingServiceRepository(RatingServiceDAO rateDAO)
        {
            _rateDAO = rateDAO;
        }
        public async Task<ServiceRating> AddRating(ServiceRating item)
        {
            return await _rateDAO.AddRating(item);
        }
    }
}
