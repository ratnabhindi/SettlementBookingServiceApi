using Configurations;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Services.DTOs;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BookingService(IBookingRepository bookingRepository, IBookingOptionsService bookingOptionsService, IApiLogger<BookingService> logger) : IBookingService
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;
        private readonly IBookingOptionsService _bookingOptionsService = bookingOptionsService;
        private readonly IApiLogger<BookingService> _logger = logger;

        public async Task<BookingResponse?> AddBookingAsync(BookingRequest request)
        {
            if (string.IsNullOrEmpty(request.BookingTime) || string.IsNullOrEmpty(request.Name))            
                return null;

            
            DateTime bookingTime = DateTime.Today.Add(TimeSpan.Parse(request.BookingTime));
            if (!IsBookingTimeValid(bookingTime))
            {
                _logger.LogWarning("Booking time {BookingTime} is out of valid business hours.", bookingTime);
                throw new InvalidOperationException($"Booking time must be within business hours ({_bookingOptionsService.GetBookingOptions().StartHour:hh\\:mm} - {_bookingOptionsService.GetBookingOptions().EndHour:hh\\:mm}).");
            }

            if (!await IsTimeSlotAvailableAsync(bookingTime))
            {
                _logger.LogWarning("Failed to add booking as no time slots are available at {BookingTime}.", bookingTime);
                throw new BookingUnavailableException("Time slot not available.");
            }

            var booking = new Booking { Name = request.Name, BookingTime = bookingTime, Id = Guid.NewGuid() };
            await _bookingRepository.AddBookingAsync(booking);

            _logger.LogInformation("Booking {BookingId} added successfully.", booking.Id);

            return new BookingResponse { BookingId = booking.Id };
        }

        private bool IsBookingTimeValid(DateTime bookingTime)
        {
            BookingOptions options = _bookingOptionsService.GetBookingOptions();
            TimeSpan lastValidStartTime = options.EndHour - options.BookingDuration;
            return bookingTime.TimeOfDay >= options.StartHour && bookingTime.TimeOfDay <= lastValidStartTime;
        }

        private async Task<bool> IsTimeSlotAvailableAsync(DateTime bookingTime)
        {
            int bookingsAtThisTime = await _bookingRepository.GetBookingsCountAsync(bookingTime);
            return bookingsAtThisTime < _bookingOptionsService.GetBookingOptions().MaxSimultaneousBookings;
        }

    }
}
