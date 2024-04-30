using Microsoft.AspNetCore.Mvc;
using InkCanvas.Models;
using System.Collections.Generic;
using System.Linq;

namespace InkCanvas.Controllers
{
    public class ChartController : Controller
    {
        private readonly CloneIdentityContext _context;

        public ChartController(CloneIdentityContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Chart data for user age distribution.
            var users = _context.Users.ToList();
            var ageDistribution = new Dictionary<string, int>
            {
                { "До 18", users.Count(u => u.Age < 18) },
                { "18-25", users.Count(u => u.Age >= 18 && u.Age <= 25) },
                { "26-35", users.Count(u => u.Age >= 26 && u.Age <= 35) },
                { "36-45", users.Count(u => u.Age >= 36 && u.Age <= 45) },
                { "46-55", users.Count(u => u.Age >= 46 && u.Age <= 55) },
                { "56 і старші", users.Count(u => u.Age >= 56) }
            };
            var userChartData = ageDistribution.Select(pair => new object[] { pair.Key, pair.Value }).ToList();
            userChartData.Insert(0, new object[] { "Вікова категорія", "Кількість користувачів" });

            // Chart data for like distribution of the 10 most liked posts.
            var mostLikedPosts = _context.Posts
                .OrderByDescending(p => _context.Likes.Count(l => l.PostId == p.PostId))
                .Take(10)
                .ToList();

            var likeDistribution = mostLikedPosts
                .Select(p => _context.Likes.Count(l => l.PostId == p.PostId))
                .ToList();

            // Modifying the like distribution data to include post names and IDs.
            var likeChartData = mostLikedPosts.Select((post, index) => new object[] { $"{post.Caption} (PostId: {post.PostId})", likeDistribution[index] }).ToList();
            likeChartData.Insert(0, new object[] { "Пост", "Кількість вподобань" });

            ViewBag.UserChartData = userChartData;
            ViewBag.LikeChartData = likeChartData;

            return View();
        }
    }
}
