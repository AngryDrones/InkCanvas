using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InkCanvas.Models;
using Microsoft.AspNetCore.Authorization;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;
using System.Security.Claims;

namespace InkCanvas.Controllers
{
    [Authorize(Roles="user,admin")]
    public class PostsController : Controller
    {
        private readonly CloneIdentityContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostsController(IWebHostEnvironment hostEnvironment, CloneIdentityContext context)
        {
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var cloneIdentityContext = _context.Posts.Include(p => p.User);
            return View(await cloneIdentityContext.ToListAsync());
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
                .Include(p => p.Comments) // Include comments
                .Include(post => post.Likes) // and likes
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PostId,Caption,Description")] Post post)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //        post.UserId = userId;

        //        DateTime date = DateTime.Now;
        //        post.Date = date;

        //        _context.Add(post);
        //        await _context.SaveChangesAsync();

        //        return RedirectToAction(nameof(Index));
        //    }
        //    //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);

        //    return View(post);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Caption,Description")] Post post, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                        var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        post.ImageUrl = "/uploads/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("ImageFile", $"Image upload failed: {ex.Message}");
                        return View(post);
                    }
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.UserId = userId;
                post.Date = DateTime.Now;

                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,UserId,Caption,Description")] Post post)
        {
            if (id != post.PostId)
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
                    if (!PostExists(post.PostId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
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
                .FirstOrDefaultAsync(m => m.PostId == id);
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

            // Remove comments and likes.
            var relatedComments = _context.Comments.Where(comment => comment.PostId == id);
            _context.Comments.RemoveRange(relatedComments);

            var relatedLikes = _context.Likes.Where(like => like.PostId == id);
            _context.Likes.RemoveRange(relatedLikes);

            // Remove post.
            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }

        // ALL posts view.
        public async Task<IActionResult> AllPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return View(posts);
        }

        // Search post by caption.
        public async Task<IActionResult> SearchPost(string searchString)
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

            return View("./Views/PostSearch/SearchPost.cshtml", posts);
        }

        public IActionResult NoResultsFound(string message)
        {
            ViewBag.Message = message;
            return View("./Views/PostSearch/NoResultsFound.cshtml");
        }

        [HttpPost]
        public IActionResult Upload(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                try
                {
                    // Generate a unique file name
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                    // Get the path where you want to store the image
                    var uploadPath = Path.Combine(_hostEnvironment.WebRootPath, "uploads");

                    // Ensure the upload directory exists
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Combine the upload path and file name
                    var filePath = Path.Combine(uploadPath, fileName);

                    // Save the file to the server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }

                    // Notify the user that upload was successful
                    TempData["Message"] = "Upload successful!";
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the upload process
                    TempData["ErrorMessage"] = $"Upload failed: {ex.Message}";
                }
            }
            else
            {
                // Handle the case where no file was uploaded
                TempData["ErrorMessage"] = "Please select a file to upload.";
            }

            return RedirectToAction("Index"); // Redirect to Index action after upload attempt
        }

    }
}
