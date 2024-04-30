using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class BookingRepository :IBookingRepository
    {
        private readonly List<Booking> _bookings = new List<Booking>();

        public Task<Booking> AddBookingAsync(Booking booking)
        {
            _bookings.Add(booking);
            return Task.FromResult(booking);
        }

        public Task<Booking> GetBookingByIdAsync(Guid bookingId)
        {
            Booking booking = _bookings.FirstOrDefault(b => b.Id == bookingId);
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
