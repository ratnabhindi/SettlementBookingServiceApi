using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Booking
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime BookingTime { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateCreated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateModified { get; set; }

        // Constructor to initialize DateCreated and DateModified
        public Booking()
        {
            DateCreated = DateTime.UtcNow;
            DateModified = DateTime.UtcNow;
        }
    }
}
