using Microsoft.AspNetCore.Mvc;

namespace _1.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
