﻿using BookingService.Models;

namespace BookingService.Services.Interface
{
    public interface IServicingService
    {
        // Service
        Task<IEnumerable<BookingService.Models.Service>> GetAllService();
        Task<IEnumerable<BookingService.Models.Service>> GetAllServiceAvailable();
        Task<BookingService.Models.Service> GetServiceById(int id);
        Task<IEnumerable<BookingService.Models.Service>> GetServicesPaged(int pageNumber, int pageSize);
        Task<int> GetTotalServicesCount();
        Task<IEnumerable<BookingService.Models.Service>> GetAllServiceByAccId(int id);
        Task<BookingService.Models.Service> AddService(BookingService.Models.Service item);
        Task<BookingService.Models.Service> UpdateService(BookingService.Models.Service item);
        Task DeleteService(int id);
        // Rating
        Task<ServiceRating> AddRating(ServiceRating item);
        Task<IEnumerable<ServiceRating>> GetAllRating();
        Task<ServiceRating> GetRatingById(int id);
        Task<ServiceRating> UpdateRating(ServiceRating item);
        Task<IEnumerable<ServiceRating>> GetAllRatingByServiceId(int id);
        // CategoryService
        Task<CategoryService> GetCategoryServiceById(int id);
        Task<IEnumerable<CategoryService>> GetAllCategoryService();
        Task AddCategoryService(CategoryService item);
        Task UpdateCategoryService(CategoryService item);
        Task DeleteCategoryService(int id);
        // Booking
        Task<IEnumerable<BookingService.Models.BookingService>> GetAllBooking();
        Task<BookingService.Models.BookingService> GetBookingById(int id);
        Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item);
        Task UpdateStatusBooking(BookingService.Models.BookingService item);
        Task<int> CountBookingConfirmBySerId(int id);
        Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingByAccId(int id);
        Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingBySerId(int id);
    }
}
