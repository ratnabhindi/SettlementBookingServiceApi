namespace Configurations
{
    public class BookingOptions
    {
        public int MaxSimultaneousBookings { get; set; }
        public TimeSpan BookingDuration { get; set; }
        public TimeSpan StartHour { get; set; } 
        public TimeSpan EndHour { get; set; } 

    }
}
