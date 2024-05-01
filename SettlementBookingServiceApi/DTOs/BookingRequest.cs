using System.ComponentModel.DataAnnotations;

namespace WebApi.DTOs
{
    public class BookingRequest
    {
        [Required(ErrorMessage = "Booking time is required.")]
        public required string BookingTime { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }
    }
}
