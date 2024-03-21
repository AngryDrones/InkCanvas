using ImagiArtInfrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ImagiArtInfrastructure.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloneContext _context;

        public HomeController(CloneContext context)
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
