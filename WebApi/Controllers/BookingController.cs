using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Domain.Exceptions;
using Services.DTOs;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    public class BookingController(IBookingService bookingService, IApiLogger<BookingController> logger) : ControllerBase
    {
        private readonly IBookingService _bookingService = bookingService;
        private readonly IApiLogger<BookingController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] BookingRequest request)
        {
            _logger.LogInformation("Received booking request.");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _bookingService.AddBookingAsync(request);
                return Ok(response);
            }
            catch (BookingUnavailableException ex)
            {
                _logger.LogError(ex, "Failed to process booking request.");
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");
                return BadRequest("An unexpected error occurred.");
            }
        }

    }
}
