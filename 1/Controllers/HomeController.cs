using InkCanvas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace InkCanvas.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloneIdentityContext _context;

        public HomeController(CloneIdentityContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cloneContext = _context.Posts.Include(p => p.User);

            //return View();
            return View(await cloneContext.ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
