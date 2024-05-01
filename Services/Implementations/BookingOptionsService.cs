using Configurations;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
