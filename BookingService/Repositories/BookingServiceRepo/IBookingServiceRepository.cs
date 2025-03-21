﻿using BookingService.Models;

namespace BookingService.Repositories.BookingServiceRepo
{
    public interface IBookingServiceRepository
    {
        Task<IEnumerable<BookingService.Models.BookingService>> GetAllBooking();
        Task<BookingService.Models.BookingService> GetBookingById(int id);
        Task<BookingService.Models.BookingService> AddBooking(BookingService.Models.BookingService item);
        Task UpdateStatusBooking(BookingService.Models.BookingService item);
        Task<int> CountBookingConfirmBySerId(int id);
        Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingByAccId(int id);
        Task<IEnumerable<BookingService.Models.BookingService>> GetAllBookingBySerId(int id);
    }
}
