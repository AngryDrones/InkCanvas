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
    public class UserFollowersController : Controller
    {
        private readonly CloneContext _context;

        public UserFollowersController(CloneContext context)
        {
            _context = context;
        }

        // GET: UserFollowers
        public async Task<IActionResult> Index()
        {
            var cloneContext = _context.UserFollowers.Include(u => u.User);
            return View(await cloneContext.ToListAsync());
        }

        // GET: UserFollowers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userFollower = await _context.UserFollowers
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFollower == null)
            {
                return NotFound();
            }

            return View(userFollower);
        }

        // GET: UserFollowers/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password");
            return View();
        }

        // POST: UserFollowers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,FollowerId,Id")] UserFollower userFollower)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userFollower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", userFollower.UserId);
            return View(userFollower);
        }

        // GET: UserFollowers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userFollower = await _context.UserFollowers.FindAsync(id);
            if (userFollower == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", userFollower.UserId);
            return View(userFollower);
        }

        // POST: UserFollowers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FollowerId,Id")] UserFollower userFollower)
        {
            if (id != userFollower.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userFollower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserFollowerExists(userFollower.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Password", userFollower.UserId);
            return View(userFollower);
        }

        // GET: UserFollowers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userFollower = await _context.UserFollowers
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userFollower == null)
            {
                return NotFound();
            }

            return View(userFollower);
        }

        // POST: UserFollowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userFollower = await _context.UserFollowers.FindAsync(id);
            if (userFollower != null)
            {
                _context.UserFollowers.Remove(userFollower);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserFollowerExists(int id)
        {
            return _context.UserFollowers.Any(e => e.Id == id);
        }
    }
}
