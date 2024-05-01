using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Domain.Models;
using WebApi.Controllers;
using WebApi.DTOs;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Configurations;

namespace WebApi.Tests.Controllers
{
    [TestFixture]
    public class BookingControllerTests
    {
        private BookingController _controller;
        private Mock<IBookingService> _mockBookingService;
        private BookingOptions _bookingOptions;

        [SetUp]
        public void Setup()
        {
            // Mock the booking service
            _mockBookingService = new Mock<IBookingService>();

            // Setup booking options
            _bookingOptions = new BookingOptions
            {
                StartHour = TimeSpan.FromHours(9),
                EndHour = TimeSpan.FromHours(17)
            };
            IOptions<BookingOptions> options = Options.Create(_bookingOptions);

            // Initialize the controller with the mocked service and options
            _controller = new BookingController(_mockBookingService.Object, options);
        }

        [Test]
        public async Task CreateBookingAsync_Returns_BadRequest_When_BookingDetails_AreNull()
        {
            // Arrange
            BookingRequest? request = null;

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateBookingAsync_Returns_BadRequest_When_Name_IsMissing()
        {
            // Arrange
            var request = new BookingRequest { Name = "", BookingTime = "10:00" };

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateBookingAsync_Returns_BadRequest_For_Invalid_TimeFormat()
        {
            // Arrange
            var request = new BookingRequest { Name = "John Doe", BookingTime = "Invalid time format" };

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateBookingAsync_Returns_BadRequest_When_BookingTime_IsOutOfBusinessEndHours()
        {
            // Arrange
            var request = new BookingRequest { Name = "John Doe", BookingTime = "18:00" }; // Time after business hours end

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "The booking time is after business hours, should return BadRequest.");
        }

        [Test]
        public async Task CreateBookingAsync_Returns_BadRequest_When_BookingTime_IsOutOfBusinessStartHours()
        {
            // Arrange
            var request = new BookingRequest { Name = "John Doe", BookingTime = "08:00" }; // Time before business hours start

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>(), "The booking time is before business hours, should return BadRequest.");
        }



        [Test]
        public async Task CreateBookingAsync_Returns_Ok_When_Booking_IsSuccessful()
        {
            // Arrange
            var request = new BookingRequest { Name = "John Doe", BookingTime = "10:00" };
            var booking = new Booking { Name = "John Doe", BookingTime = DateTime.Today.AddHours(10) };
            _mockBookingService.Setup(service => service.AddBookingAsync(It.IsAny<Booking>()))
                               .ReturnsAsync(booking);

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(okResult.StatusCode, Is.EqualTo(200));
                Assert.That(okResult.Value, Is.Not.Null);
            });
        }

        [Test]
        public async Task CreateBookingAsync_Returns_Conflict_When_ServiceThrowsArgumentException()
        {
            // Arrange
            var request = new BookingRequest { Name = "John Doe", BookingTime = "10:00" };
            _mockBookingService.Setup(service => service.AddBookingAsync(It.IsAny<Booking>()))
                               .ThrowsAsync(new ArgumentException("Error processing request"));

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
        }
    }
}