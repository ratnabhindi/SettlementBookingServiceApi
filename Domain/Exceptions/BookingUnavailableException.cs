using System;

namespace Domain.Exceptions
{   public class BookingUnavailableException(string message) : Exception(message)
    {
    }
}
