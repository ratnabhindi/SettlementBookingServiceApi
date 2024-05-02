using Domain.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Tests
{
    [TestFixture]
    public class BookingRepositoryTests
    {
        private BookingRepository _repository;
        private Booking _testBooking;

        [SetUp]
        public void Setup()
        {
            _repository = new BookingRepository();
            _testBooking = new Booking
            {
                Id = Guid.NewGuid(),
                BookingTime = DateTime.Now,
                Name = "Test Booking"
            };

            _repository.AddBookingAsync(_testBooking).Wait(); // Initial setup to add a booking
        }

        [Test]
        public async Task AddBookingAsync_StoresBookingCorrectly()
        {
            var newBooking = new Booking
            {
                Id = Guid.NewGuid(),
                BookingTime = DateTime.Now.AddHours(1),
                Name = "Another Test Booking"
            };

            await _repository.AddBookingAsync(newBooking);
            var retrievedBooking = await _repository.GetBookingByIdAsync(newBooking.Id);

            Assert.Multiple(() =>
            {
                Assert.That(retrievedBooking?.Id, Is.EqualTo(newBooking.Id));
                Assert.That(retrievedBooking?.Name, Is.EqualTo(newBooking.Name));
                Assert.That(retrievedBooking?.BookingTime, Is.EqualTo(newBooking.BookingTime));
            });
        }

        [Test]
        public async Task GetBookingByIdAsync_ReturnsCorrectBooking()
        {
            var retrievedBooking = await _repository.GetBookingByIdAsync(_testBooking.Id);

            Assert.Multiple(() =>
            {
                Assert.That(retrievedBooking, Is.Not.Null);
                Assert.That(retrievedBooking?.Id, Is.EqualTo(_testBooking.Id));
            });
        }

        [Test]
        public async Task GetBookingsCountAsync_ReturnsCorrectCount()
        {
            var bookingTime = _testBooking.BookingTime;
            var count = await _repository.GetBookingsCountAsync(bookingTime);

            Assert.That(count, Is.EqualTo(1)); 
        }

        [Test]
        public async Task GetAllBookingsAsync_ReturnsAllBookings()
        {
            var bookings = await _repository.GetAllBookingsAsync();
            var bookingsList = bookings.ToList();

            Assert.That(bookingsList, Does.Contain(_testBooking));
            Assert.That(bookingsList, Has.Count.EqualTo(1));
        }
    }
}
