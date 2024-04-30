using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class BookingController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
