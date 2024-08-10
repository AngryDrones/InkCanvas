using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using InkCanvas.ViewModel;
using InkCanvas.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace InkCanvas.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly CloneIdentityContext _context;
        public AccountController(UserManager<User> userManager,
            SignInManager<User> signInManager,
            CloneIdentityContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    Login = model.Login,
                    UserName = model.Email,
                    Email = model.Email,
                    Age = model.Age
                };

                // Adding the user.
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    // Cookies.
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }
        //[HttpGet]
        //public IActionResult Login(string? returnUrl = null)
        //{
        //    returnUrl ??= "/"; // Set a default returnUrl to "/" if it's null
        //    return View(new LoginViewModel { ReturnUrl = returnUrl });
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid credentials");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            // Delete cookies.
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // Logged-in user's profile.
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Find user.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Include posts.
            var userWithPosts = await _context.Users
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return View(userWithPosts);
        }

        // Other users' profiles.
        [HttpGet]
        public async Task<IActionResult> UserProfile(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is visiting their own profile
            if (userId == User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Profile");
            }

            // Include posts.
            var userWithPosts = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.FollowFollowers) // include user's followers
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return View(userWithPosts);
        }

        // Update username (login)
        [HttpPost]
        public async Task<IActionResult> Updatelogin(string newLogin)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(newLogin))
            {
                currentUser.Login = newLogin;
                var result = await _userManager.UpdateAsync(currentUser);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return RedirectToAction("Profile");
        }
    }
}