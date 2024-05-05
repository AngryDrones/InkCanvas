using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InkCanvas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace InkCanvas.Controllers
{
    //[Authorize(Roles = "admin")]
    public class FollowsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly CloneIdentityContext _context;

        public FollowsController(UserManager<User> userManager, CloneIdentityContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFollow(string userId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var userToFollow = await _userManager.FindByIdAsync(userId);

            if (currentUser == null || userToFollow == null)
            {
                return NotFound();
            }

            // Check if the current user is already following the userToFollow
            var isFollowing = await _context.Follows
                .AnyAsync(f => f.UserId == userToFollow.Id && f.FollowerId == currentUser.Id);

            if (isFollowing)
            {
                // Unfollow the user
                var follow = await _context.Follows
                    .FirstOrDefaultAsync(f => f.UserId == userToFollow.Id && f.FollowerId == currentUser.Id);

                if (follow != null)
                {
                    _context.Follows.Remove(follow);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // Follow the user
                var follow = new Follow
                {
                    UserId = userToFollow.Id,
                    FollowerId = currentUser.Id
                };

                _context.Follows.Add(follow);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        // Action to display the followers of a specific user
        public async Task<IActionResult> UserFollowers(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                // If the user is not found, return a not found result
                return NotFound();
            }

            // Get the followers of the specified user
            var userFollowers = await _context.Follows
                .Where(f => f.UserId == userId)
                .Select(f => f.Follower)
                .ToListAsync();

            return View(userFollowers);
        }

        // GET: Follows
        public async Task<IActionResult> Index()
        {
            var cloneIdentityContext = _context.Follows.Include(f => f.Follower).Include(f => f.User);
            return View(await cloneIdentityContext.ToListAsync());
        }

        // GET: Follows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follows
                .Include(f => f.Follower)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FollowId == id);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // GET: Follows/Create
        public IActionResult Create()
        {
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Follows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FollowId,UserId,FollowerId")] Follow follow)
        {
            if (ModelState.IsValid)
            {
                _context.Add(follow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", follow.UserId);
            return View(follow);
        }

        // GET: Follows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follows.FindAsync(id);
            if (follow == null)
            {
                return NotFound();
            }
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", follow.UserId);
            return View(follow);
        }

        // POST: Follows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FollowId,UserId,FollowerId")] Follow follow)
        {
            if (id != follow.FollowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(follow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FollowExists(follow.FollowId))
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
            ViewData["FollowerId"] = new SelectList(_context.Users, "Id", "Id", follow.FollowerId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", follow.UserId);
            return View(follow);
        }

        // GET: Follows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var follow = await _context.Follows
                .Include(f => f.Follower)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FollowId == id);
            if (follow == null)
            {
                return NotFound();
            }

            return View(follow);
        }

        // POST: Follows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var follow = await _context.Follows.FindAsync(id);
            if (follow != null)
            {
                _context.Follows.Remove(follow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FollowExists(int id)
        {
            return _context.Follows.Any(e => e.FollowId == id);
        }
    }
}
