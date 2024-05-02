using Domain.Models;

namespace Domain.Interfaces
{
    public interface IBookingRepository
    {
        Task<Booking?> GetBookingByIdAsync(Guid bookingId);
        public Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<Booking> AddBookingAsync(Booking booking);       
        Task<int> GetBookingsCountAsync(DateTime bookingTime);
    }
}
