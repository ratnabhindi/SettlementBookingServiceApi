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
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetAllBookingsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Booking> GetBookingByIdAsync(Guid bookingId)
        {
            Booking booking = _bookings.FirstOrDefault(b =>b.Id == bookingId);
            return Task.FromResult(booking);
        }

        public Task<bool> IsBookingAvailableAsync(DateTime bookingTime)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Booking>> IBookingRepository.GetAllBookingsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
