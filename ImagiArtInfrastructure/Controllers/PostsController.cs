using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ImagiArtDomain.Model;
using ImagiArtInfrastructure;

namespace ImagiArtInfrastructure.Controllers
{
    public class PostsController : Controller
    {
        private readonly CloneContext _context;

        public PostsController(CloneContext context)
        {
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index(int? id, string? username)
        {
            var cloneContext = _context.Posts.Include(p => p.User);

            //if (id == null) return RedirectToAction("Users", "Index");
            //ViewBag.Id = id;
            //ViewBag.Username = username;
            //var postsByUser = _context.Posts.Where(b => b.UserId == id).Include(b => b.User);

            return View(await cloneContext.ToListAsync());
            //return View(await postsByUser.ToListAsync());
        }
        
        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments) // інклудимо коментарі
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Caption,Description,Id")] Post post)
        {
            User user = _context.Users.FirstOrDefault(c => c.Id == post.UserId);
            post.User = user;
            ModelState.Clear();
            TryValidateModel(post);

            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", post.UserId);
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Caption,Description,Id")] Post post)
        {
            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Username", post.UserId);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            // Видаляємо всі коментарі, пов'язані з цим постом
            var relatedComments = _context.Comments.Where(comment => comment.PostId == id);
            _context.Comments.RemoveRange(relatedComments);

            // Видаляємо всі лайки, пов'язані з цим постом
            var relatedLikes = _context.Likes.Where(like => like.PostId == id);
            _context.Likes.RemoveRange(relatedLikes);

            // Видаляємо сам пост
            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.Id == id);
        }

        // Перегляд ВСІХ постів
        public async Task<IActionResult> AllPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return View(posts);
        }

        // Пошук постів за назвою
        public async Task<IActionResult> Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("NoResultsFound", new { message = "Спробуйте ввести кілька букв" });
            }

            var posts = await _context.Posts
                .Where(p => p.Caption.Contains(searchString) || p.Description.Contains(searchString))
                .ToListAsync();

            if (posts == null || posts.Count == 0)
            {
                return RedirectToAction("NoResultsFound", new { message = "За Вашим запитом нічого не знайдено :(" });
            }

            return View(posts);
        }

        public IActionResult NoResultsFound(string message)
        {
            ViewBag.Message = message;
            return View();
        }

        // auugh
    }
}
