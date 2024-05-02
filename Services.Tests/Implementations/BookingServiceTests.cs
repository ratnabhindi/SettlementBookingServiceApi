using Moq;
using Domain.Interfaces;
using Services.Implementations;
using Domain.Models;
using Services.DTOs;
using Domain.Exceptions;
using Configurations;
using Services.Interfaces;

namespace Tests
{
    [TestFixture]
    public class BookingServiceTests
    {
        private BookingService _bookingService;
        private Mock<IBookingRepository> _mockRepository;
        private Mock<IBookingOptionsService> _mockOptionsService;
        private Mock<IApiLogger<BookingService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IBookingRepository>();
            _mockOptionsService = new Mock<IBookingOptionsService>();
            _mockLogger = new Mock<IApiLogger<BookingService>>();

            _bookingService = new BookingService(_mockRepository.Object, _mockOptionsService.Object, _mockLogger.Object);

            _mockOptionsService.Setup(s => s.GetBookingOptions()).Returns(new BookingOptions
            {
                StartHour = TimeSpan.FromHours(9),
                EndHour = TimeSpan.FromHours(17),
                BookingDuration = TimeSpan.FromHours(1),
                MaxSimultaneousBookings = 4
            });
        }


        [Test]
        public async Task AddBookingAsync_ValidBooking_ReturnsBookingResponse()
        {
            // Arrange
            var request = new BookingRequest
            {
                Name = "John Doe",
                BookingTime = "10:00"  // Assuming the input format and time are correct and within business hours
            };

            _mockRepository.Setup(r => r.AddBookingAsync(It.IsAny<Booking>())).ReturnsAsync((Booking booking) => booking);
            _mockRepository.Setup(r => r.GetBookingsCountAsync(It.IsAny<DateTime>())).ReturnsAsync(0);  // Assume no bookings at this time

            // Act
            var response = await _bookingService.AddBookingAsync(request);

            // Assert
            Assert.That(response, Is.Not.Null, "The response from AddBookingAsync should not be null.");
            Assert.That(response.BookingId, Is.Not.EqualTo(Guid.Empty), "The BookingId should not be empty.");
            _mockLogger.Verify(l => l.LogInformation(It.Is<string>(msg => msg.Contains("added successfully")), It.IsAny<Guid>()), Times.Once);
        }


        [Test]
        public void AddBookingAsync_InvalidTime_ThrowsBookingUnavailableException()
        {
            var request = new BookingRequest { Name = "John Doe", BookingTime = "18:00" };

            var ex = Assert.ThrowsAsync<BookingUnavailableException>(() => _bookingService.AddBookingAsync(request));
            Assert.That(ex.Message, Is.EqualTo("Booking time must be within business hours (09:00 - 17:00)."));
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }

        [Test]
        public void AddBookingAsync_FullSlot_ThrowsBookingUnavailableException()
        {
            var request = new BookingRequest { Name = "John Doe", BookingTime = "10:00" };
            _mockRepository.Setup(r => r.GetBookingsCountAsync(It.IsAny<DateTime>())).ReturnsAsync(4);

            var ex = Assert.ThrowsAsync<BookingUnavailableException>(() => _bookingService.AddBookingAsync(request));
            Assert.That(ex.Message, Is.EqualTo("Time slot not available."));
            _mockLogger.Verify(l => l.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
        }
    }
}
