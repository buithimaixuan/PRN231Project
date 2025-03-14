using BookingService.DAOs;
using BookingService.Models;
using RequestService.DAOs;

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
        public async Task<IEnumerable<ServiceRating>> GetAllRating()
        {
            return await _rateDAO.GetAllRating();
        }
        public async Task<ServiceRating> GetRatingById(int id)
        {
            return await _rateDAO.GetRatingById(id);
        }
        public async Task<ServiceRating> UpdateRating(ServiceRating item)
        {
            return await _rateDAO.UpdateRating(item);
        }
    }
}
