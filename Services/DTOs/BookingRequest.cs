using System.ComponentModel.DataAnnotations;

namespace Services.DTOs
{
    public class BookingRequest
    {
        [Required]
        [RegularExpression(@"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Time must be in HH:mm format.")]
        public string? BookingTime { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
    }
}
