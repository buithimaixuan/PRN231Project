﻿using BookingService.Models;
using BookingService.Repositories.BookingServiceRepo;
using BookingService.Repositories.CategoryServiceRepo;
using BookingService.Repositories.RatingServiceRepo;
using BookingService.Repositories.ServiceRepo;
using BookingService.Services.Interface;
using RequestService.DAOs;

namespace BookingService.Services.Implement
{
    public class ServicingService : IServicingService
    {
        private IServiceRepository _servicingService;
        private IRatingServiceRepository _ratingServicingService;
        private ICategoryServiceRepository _categoryServicingService;
        private IBookingServiceRepository _bookingServicingService;
        public ServicingService(IServiceRepository servicingService, IRatingServiceRepository ratingServicingService, ICategoryServiceRepository categoryServicingService, IBookingServiceRepository bookingServicingService)
        {
            _servicingService = servicingService;
            _ratingServicingService = ratingServicingService;
            _categoryServicingService = categoryServicingService;
            _bookingServicingService = bookingServicingService;
        }
        // Service
        public async Task<IEnumerable<BookingService.Models.Service>> GetAllService()
        {
            return await _servicingService.GetAll();
        }
        public async Task<IEnumerable<BookingService.Models.Service>> GetAllServiceAvailable()
        {
            return await _servicingService.GetAllServiceAvailable();
        }
        public async Task<BookingService.Models.Service> GetServiceById(int id)
        {
            return await _servicingService.GetById(id);
        }
        public async Task<IEnumerable<BookingService.Models.Service>> GetServicesPaged(int pageNumber, int pageSize)
        {
            return await _servicingService.GetServicesPaged(pageNumber, pageSize);
        }
        public async Task<int> GetTotalServicesCount()
        {
            return await _servicingService.GetTotalServicesCount();
        }
        public async Task<IEnumerable<BookingService.Models.Service>> GetAllServiceByAccId(int id)
        {
            return await _servicingService.GetAllServiceByAccId(id);
        }
        public async Task<BookingService.Models.Service> AddService(BookingService.Models.Service item)
        {
            return await _servicingService.AddService(item);
        }
        public async Task<BookingService.Models.Service> UpdateService(BookingService.Models.Service item)
        {
            return await _servicingService.UpdateService(item);
        }
        public async Task DeleteService(int id)
        {
            await _servicingService.DeleteService(id);
        }
        // Rating
        public async Task<ServiceRating> AddRating(ServiceRating item)
        {
            return await _ratingServicingService.AddRating(item);
        }
        public async Task<IEnumerable<ServiceRating>> GetAllRating()
        {
            return await _ratingServicingService.GetAllRating();
        }
        public async Task<ServiceRating> GetRatingById(int id)
        {
            return await _ratingServicingService.GetRatingById(id);
        }
        public async Task<ServiceRating> UpdateRating(ServiceRating item)
        {
            return await _ratingServicingService.UpdateRating(item);
        }
        public async Task<IEnumerable<ServiceRating>> GetAllRatingByServiceId(int id)
        {
            return await _ratingServicingService.GetAllRatingByServiceId(id);
        }
        // CategoryService
        public async Task<IEnumerable<CategoryService>> GetAllCategoryService() => await _categoryServicingService.GetAllCategoryService();

        public async Task<CategoryService> GetCategoryServiceById(int id) => await _categoryServicingService.GetCategoryServiceById(id);
        public async Task AddCategoryService(CategoryService item)
        {
            await _categoryServicingService.AddCategoryService(item);
        }
        public async Task UpdateCategoryService(CategoryService item)
        {
            await _categoryServicingService.UpdateCategoryService(item);
        }
        public async Task DeleteCategoryService(int id)
        {
            await _categoryServicingService.DeleteCategoryService(id);
        }
        // Booking
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBooking()
        {
            return await _bookingServicingService.GetAllBooking();
        }
        public async Task<BookingService.Models.BookingService> GetBookingById(int id)
        {
            return await _bookingServicingService.GetBookingById(id);
        }
        public async Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item)
        {
            return await _bookingServicingService.AddBooking(item);
        }
        public async Task UpdateStatusBooking(BookingService.Models.BookingService item)
        {
            await _bookingServicingService.UpdateStatusBooking(item);
        }
        public async Task<int> CountBookingConfirmBySerId(int id)
        {
            return await _bookingServicingService.CountBookingConfirmBySerId(id);
        }
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingByAccId(int id)
        {
            return await _bookingServicingService.GetAllBookingByAccId(id);
        }
        public async Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingBySerId(int id)
        {
            return await _bookingServicingService.GetAllBookingBySerId(id);
        }
    }
}
