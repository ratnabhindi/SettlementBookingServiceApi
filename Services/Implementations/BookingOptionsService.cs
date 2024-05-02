using Configurations;
using Microsoft.Extensions.Options;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BookingOptionsService : IBookingOptionsService
    {
        private readonly BookingOptions _options;

        public BookingOptionsService(IOptions<BookingOptions> options)
        {
            _options = options.Value;
        }

        public BookingOptions GetBookingOptions() => _options;
    }
}
