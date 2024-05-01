using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using WebApi.DTOs;
using System;
using Microsoft.Extensions.Options;
using Configurations;
using Microsoft.Extensions.Logging;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly BookingOptions _bookingOptions;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, IOptions<BookingOptions> bookingOptions, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _bookingOptions = bookingOptions.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] BookingRequest request)
        {
            _logger.LogInformation("Received booking request.");

            if (request == null)
            {
                return BadRequest("Booking details must be provided.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Booking name must be provided and it cannot be empty.");
            }

            if (!TimeSpan.TryParse(request.BookingTime, out TimeSpan bookingTimeSpan))
            {
                _logger.LogWarning("Invalid booking time format.");
                return BadRequest("Invalid time format. Please provide time in HH:mm format.");
            }

            var bookingDateTime = DateTime.Today.Add(bookingTimeSpan);
            if (!IsBookingTimeValid(bookingDateTime))
            {
                _logger.LogWarning("Booking time {BookingTime} is out of valid business hours.", bookingDateTime);
                return BadRequest($"Booking time must be within business hours ({_bookingOptions.StartHour:h\\:mm} - {_bookingOptions.EndHour:h\\:mm}).");
            }

            try
            {
                var booking = new Booking { Name = request.Name, BookingTime = bookingDateTime };
                var addedBooking = await _bookingService.AddBookingAsync(booking);
                _logger.LogInformation("Booking {BookingId} created successfully at {BookingTime}.", addedBooking.Id, bookingDateTime);
                return Ok(new { bookingId = addedBooking.Id });
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Error while creating booking.");
                return Conflict(ex.Message);
            }
        }

        private bool IsBookingTimeValid(DateTime bookingTime)
        {
            TimeSpan lastValidStartTime = _bookingOptions.EndHour - _bookingOptions.BookingDuration;
            return bookingTime.TimeOfDay >= _bookingOptions.StartHour && bookingTime.TimeOfDay <= lastValidStartTime;
        }
    }
}
