using Configurations;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingOptions _bookingOptions;

        public BookingService(IBookingRepository bookingRepository, IOptions<BookingOptions> bookingOptions)
        {
            _bookingRepository = bookingRepository;
            _bookingOptions = bookingOptions.Value ;
        }

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            if (booking == null || string.IsNullOrWhiteSpace(booking.Name) || !await IsTimeSlotAvailableAsync(booking.BookingTime))
            {
                throw new ArgumentException("Invalid booking details or time slot not available.");
            }

            booking.Id = Guid.NewGuid();
            await _bookingRepository.AddBookingAsync(booking);
            return booking;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(DateTime bookingTime)
        {
            int bookingsAtThisTime = await _bookingRepository.GetBookingsCountAsync(bookingTime);
            return bookingsAtThisTime < _bookingOptions.MaxSimultaneousBookings;
        }
    }
}
