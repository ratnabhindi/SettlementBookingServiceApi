﻿using Domain.Models;
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
    [ApiVersion("1.0")]
    public class BookingController(IBookingService bookingService, IBookingOptionsService bookingOptionsService, IApiLogger<BookingController> logger) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly IBookingOptionsService _bookingOptionsService = bookingOptionsService;
        private readonly IApiLogger<BookingController> _logger = logger;

        [HttpPost]
        [MapToApiVersion("1.0")]
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

            DateTime bookingDateTime = DateTime.Today.Add(bookingTimeSpan);
            if (!IsBookingTimeValid(bookingDateTime))
            {
                _logger.LogWarning("Booking time {BookingTime} is out of valid business hours.", bookingDateTime);
                return BadRequest($"Booking time must be within business hours ({_bookingOptionsService.GetBookingOptions().StartHour:h\\:mm} - {_bookingOptionsService.GetBookingOptions().EndHour:h\\:mm}).");
            }

            try
            {
                Booking booking = new() { Name = request.Name, BookingTime = bookingDateTime };
                Booking addedBooking = await _bookingService.AddBookingAsync(booking);
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
            TimeSpan lastValidStartTime = _bookingOptionsService.GetBookingOptions().EndHour - _bookingOptionsService.GetBookingOptions().BookingDuration;
            return bookingTime.TimeOfDay >= _bookingOptionsService.GetBookingOptions().StartHour && bookingTime.TimeOfDay <= lastValidStartTime;
        }
    }
}
