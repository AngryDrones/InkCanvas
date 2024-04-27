﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InkCanvas.Models;
using Microsoft.AspNetCore.Authorization;

namespace InkCanvas.Controllers
{
    [Authorize(Roles = "admin")]
    public class PostsController : Controller
    {
        private readonly CloneIdentityContext _context;

        public PostsController(CloneIdentityContext context)
        {
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,UserId,Caption")] Post post)
        {
            if (ModelState.IsValid)
            {
                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", post.UserId);
            //// Retrieve usernames and IDs from the database
            //var users = _context.Users.Select(u => new { Id = u.Id, UserName = u.UserName }).ToList();

            //// Create a SelectList using usernames as the display text and IDs as the values
            //var userSelectList = new SelectList(users, "UserName", "UserName");

            //// Pass the SelectList to the view
            //ViewBag.UserName = userSelectList;

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
        public async Task<IActionResult> Edit(int id, [Bind("PostId,UserId,Caption")] Post post)
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

        // Search post by caption (no description for now).
        public async Task<IActionResult> SearchPost(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("NoResultsFound", new { message = "Спробуйте ввести кілька букв" });
            }

            var posts = await _context.Posts
                .Where(p => p.Caption.Contains(searchString)/* || p.Description.Contains(searchString)*/)
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
    }
}
