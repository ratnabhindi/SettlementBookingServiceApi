using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using Domain.Interfaces;
using Domain.Models;
using Services.Implementations;
using System;
using System.Threading.Tasks;
using Configurations;
using Serilog;
using Microsoft.Extensions.Logging;
using Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Services.Tests.Implementations
{
    [TestFixture]
    public class BookingServiceTests
    {
        private BookingService _service;
        private Mock<IBookingRepository> _mockRepository;
        private Mock<IBookingOptionsService> _mockBookingOptionsService;
        private Mock<IApiLogger<BookingService>> _mockLogger;
        private BookingOptions _bookingOptions;

        [SetUp]
        public void SetUp()
        {
            // Initialize mocks
            _mockRepository = new Mock<IBookingRepository>();
            _mockBookingOptionsService = new Mock<IBookingOptionsService>();
            _mockLogger = new Mock<IApiLogger<BookingService>>();

            // Setup BookingOptions
            _bookingOptions = new BookingOptions { MaxSimultaneousBookings = 3 };
            _mockBookingOptionsService.Setup(service => service.GetBookingOptions()).Returns(_bookingOptions);

            // Create instance of BookingService with mocked dependencies
            _service = new BookingService(_mockRepository.Object,
                                          _mockBookingOptionsService.Object,
                                          _mockLogger.Object);
        }

        [Test]
        public void AddBookingAsync_Throws_ArgumentException_For_Invalid_Booking()
        {
            // Arrange
            Booking? booking = null;


            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.AddBookingAsync(booking));
            Assert.That(ex.Message, Is.EqualTo("Invalid booking details."));
        }

        [Test]
        public async Task AddBookingAsync_Adds_Booking_When_Valid()
        {
            // Arrange
            var booking = new Booking { Name = "Valid Booking", BookingTime = DateTime.Now };
            _mockRepository.Setup(r => r.GetBookingsCountAsync(It.IsAny<DateTime>()))
                           .ReturnsAsync(0);

            // Act
            var result = await _service.AddBookingAsync(booking);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Valid Booking"));
            _mockRepository.Verify(r => r.AddBookingAsync(It.IsAny<Booking>()), Times.Once);
        }

        [Test]
        public async Task IsTimeSlotAvailableAsync_Returns_False_When_Max_Bookings_Reached()
        {
            // Arrange
            var bookingTime = DateTime.Now;
            _mockRepository.Setup(r => r.GetBookingsCountAsync(bookingTime))
                           .ReturnsAsync(_bookingOptions.MaxSimultaneousBookings); // Max bookings reached

            // Act
            var result = await _service.IsTimeSlotAvailableAsync(bookingTime);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsTimeSlotAvailableAsync_Returns_True_When_Slots_Available()
        {
            // Arrange
            var bookingTime = DateTime.Now.AddHours(1); // Future time
            _mockRepository.Setup(r => r.GetBookingsCountAsync(bookingTime))
                           .ReturnsAsync(0); // No bookings at this time

            // Act
            var result = await _service.IsTimeSlotAvailableAsync(bookingTime);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsTimeSlotAvailableAsync_Returns_False_When_BookingLimit_Reached()
        {
            // Arrange
            var bookingTime = DateTime.Now;
            _mockRepository.Setup(r => r.GetBookingsCountAsync(bookingTime))
                                  .ReturnsAsync(_bookingOptions.MaxSimultaneousBookings); // Simulate limit is reached

            // Act
            var isAvailable = await _service.IsTimeSlotAvailableAsync(bookingTime);

            // Assert
            Assert.That(isAvailable, Is.False);
        }

        [Test]
        public async Task IsTimeSlotAvailableAsync_Returns_True_When_BookingLimit_Not_Reached()
        {
            // Arrange
            var bookingTime = DateTime.Now;
            _mockRepository.Setup(r => r.GetBookingsCountAsync(bookingTime))
                                  .ReturnsAsync(_bookingOptions.MaxSimultaneousBookings - 1); // Simulate limit is not yet reached

            // Act
            var isAvailable = await _service.IsTimeSlotAvailableAsync(bookingTime);

            // Assert
            Assert.That(isAvailable);
        }
    }
}