using Moq;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.DTOs;
using Domain.Exceptions;
using WebApi.Controllers;

namespace Tests
{
    [TestFixture]
    public class BookingControllerTests
    {
        private Mock<IBookingService> _mockBookingService;
        private Mock<IApiLogger<BookingController>> _mockLogger;
        private BookingController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockLogger = new Mock<IApiLogger<BookingController>>();
            _controller = new BookingController(_mockBookingService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task CreateBookingAsync_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new BookingRequest();
            _controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = await _controller.CreateBookingAsync(request);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task CreateBookingAsync_ReturnsOk_WhenBookingIsSuccessful()
        {
            var request = new BookingRequest();
            var response = new BookingResponse();
            _mockBookingService.Setup(s => s.AddBookingAsync(request)).ReturnsAsync(response);

            var result = await _controller.CreateBookingAsync(request);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.Value, Is.EqualTo(response));
        }

        [Test]
        public async Task CreateBookingAsync_ReturnsConflict_WhenBookingUnavailableExceptionThrown()
        {
            var request = new BookingRequest();
            _mockBookingService.Setup(s => s.AddBookingAsync(request)).ThrowsAsync(new BookingUnavailableException("Unavailable"));

            var result = await _controller.CreateBookingAsync(request);

            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
            var conflictResult = result as ConflictObjectResult;
            Assert.That(conflictResult?.Value, Is.EqualTo("Unavailable"));
        }

        [Test]
        public async Task CreateBookingAsync_ReturnsBadRequest_WhenUnexpectedExceptionThrown()
        {
            var request = new BookingRequest();
            _mockBookingService.Setup(s => s.AddBookingAsync(request)).ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.CreateBookingAsync(request);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value, Is.EqualTo("An unexpected error occurred."));
        }
    }
}
