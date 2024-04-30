using Configurations;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Options;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly BookingOptions _bookingOptions;

        public BookingService(IOptions<BookingOptions> bookingOptions)
        {
            _bookingOptions = bookingOptions.Value;
        }
        public Task<Booking> AddBookingAsync(Booking booking)
        {

            throw new NotImplementedException();
        }

        public Task<bool> IsTimeSlotAvailableAsync(DateTime bookingTime)
        {
            throw new NotImplementedException();
        }
    }
}
