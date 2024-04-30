using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> AddBookingAsync(Booking booking);
        Task<bool> IsTimeSlotAvailableAsync(DateTime bookingTime);
    }
}
