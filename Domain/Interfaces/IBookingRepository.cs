using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking> GetBookingByIdAsync(Guid bookingId);
        Task<List<Booking>> GetAllBookingsAsync();
        Task<Booking> AddBookingAsync(Booking booking);
    }
}
