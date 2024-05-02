using Domain.Models;
using Services.DTOs;

namespace Services.Interfaces
{
    public interface IBookingService
    {   
        Task<BookingResponse?> AddBookingAsync(BookingRequest booking);
    }
}
