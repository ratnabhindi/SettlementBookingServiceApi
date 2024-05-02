using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Implements the IBookingRepository interface with an in-memory collection.
    /// This repository is currently designed for demonstration purposes and 
    /// uses an in-memory list to store bookings.
    /// </summary>
    /// <remarks>
    /// In a production environment, this would be replaced with a database-backed repository.
    /// The use of asynchronous methods anticipates this potential upgrade, allowing for
    /// an easy transition to asynchronous database access methods which support scalability
    /// and efficient resource use in web applications.
    /// </remarks>
    /// 
    public class BookingRepository :IBookingRepository
    {
        private readonly List<Booking> _bookings = [];

        public Task<Booking> AddBookingAsync(Booking booking)
        {
            _bookings.Add(booking);
            return Task.FromResult(booking);
        }

        public Task<Booking?> GetBookingByIdAsync(Guid bookingId)
        {
            Booking? booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
            return Task.FromResult(booking);
        }

        public Task<int> GetBookingsCountAsync(DateTime bookingTime)
        {
            int count = _bookings.Count(b => b.BookingTime == bookingTime);
            return Task.FromResult(count);
        }

        public Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return Task.FromResult<IEnumerable<Booking>>(_bookings);
        }       

    }
}
