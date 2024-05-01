using Configurations;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BookingService(IBookingRepository bookingRepository, IBookingOptionsService bookingOptionsService, IApiLogger<BookingService> logger) : IBookingService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IBookingOptionsService _bookingOptionsService = bookingOptionsService;
        private readonly IApiLogger<BookingService> _logger = logger;

        public async Task<Booking> AddBookingAsync(Booking booking)
        {
            if (booking == null || string.IsNullOrWhiteSpace(booking.Name))
            {
                _logger.LogWarning("Failed to add booking due to invalid input.");
                throw new ArgumentException("Invalid booking details.");
            }

            if (!await IsTimeSlotAvailableAsync(booking.BookingTime))
            {
                _logger.LogWarning("Failed to add booking as no time slots are available.");
                throw new ArgumentException("Time slot not available.");
            }

            booking.Id = Guid.NewGuid();
            await _bookingRepository.AddBookingAsync(booking);          
            _logger.LogInformation("Booking {BookingId} added successfully.", booking.Id);

            return booking;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(DateTime bookingTime)
        {

            _logger.LogInformation("Checking availability for the booking time: {bookingTime}", bookingTime);

            int bookingsAtThisTime = await _bookingRepository.GetBookingsCountAsync(bookingTime);
            bool isAvailable = bookingsAtThisTime < _bookingOptionsService.GetBookingOptions().MaxSimultaneousBookings;
         
            _logger.LogInformation("Time slot availability at {BookingTime}: {IsAvailable}.", bookingTime, isAvailable);
            return isAvailable;
        }
    }
}
