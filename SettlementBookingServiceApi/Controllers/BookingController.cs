using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using WebApi.DTOs;
using System;
using Microsoft.Extensions.Options;
using Configurations;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController(IBookingService bookingService, IOptions<BookingOptions> bookingOptions) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly BookingOptions _bookingOptions = bookingOptions.Value;

        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] BookingRequest request)
        {
            var validationResult = ValidateBookingRequest(request);
            if (validationResult != null)
            {
                return validationResult;
            }

            TimeSpan? bookingTimeSpan = ParseTime(request.BookingTime);
            if (!bookingTimeSpan.HasValue)
            {
                return BadRequest("Invalid time format. Please provide time in HH:mm format.");
            }

            var bookingTime = DateTime.Today.Add(bookingTimeSpan.Value);
            if (!IsBookingTimeValid(bookingTime))
            {
                return BadRequest($"Booking time must be within business hours ({_bookingOptions.StartHour:h\\:mm} - {_bookingOptions.EndHour:h\\:mm}).");
            }

            try
            {
                var booking = new Booking { Name = request.Name, BookingTime = bookingTime };
                var addedBooking = await _bookingService.AddBookingAsync(booking);
                return Ok(new { bookingId = addedBooking.Id });
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }

        private BadRequestObjectResult? ValidateBookingRequest(BookingRequest request)
        {
            if (request == null)
            {
                return BadRequest("Booking details must be provided.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest("Booking name must be provided and it cannot be empty.");
            }

            return null;
        }

        private static TimeSpan? ParseTime(string time)
        {
            try
            {
                return TimeSpan.Parse(time);
            }
            catch (FormatException)
            {
                return null;
            }
        }

        private bool IsBookingTimeValid(DateTime time)
        {        
            TimeSpan lastValidStartTime = _bookingOptions.EndHour - _bookingOptions.BookingDuration;
            return time.TimeOfDay >= _bookingOptions.StartHour && time.TimeOfDay <= lastValidStartTime;
        }
      
    }
}
