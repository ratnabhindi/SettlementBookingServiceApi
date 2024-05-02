using Moq;
using Microsoft.Extensions.Options;
using Services.Implementations;
using Configurations;

namespace Tests
{
    [TestFixture]
    public class BookingOptionsServiceTests
    {
        private BookingOptionsService _service;
        private BookingOptions _expectedOptions;

        [SetUp]
        public void Setup()
        {
            _expectedOptions = new BookingOptions
            {
                StartHour = TimeSpan.FromHours(9),
                EndHour = TimeSpan.FromHours(17),
                BookingDuration = TimeSpan.FromHours(1),
                MaxSimultaneousBookings = 4
            };

            var mockOptions = new Mock<IOptions<BookingOptions>>();
            mockOptions.Setup(opt => opt.Value).Returns(_expectedOptions);

            _service = new BookingOptionsService(mockOptions.Object);
        }

        [Test]
        public void GetBookingOptions_ReturnsExpectedOptions()
        {
            var options = _service.GetBookingOptions();
            Assert.That(options, Is.EqualTo(_expectedOptions));
        }
    }
}
