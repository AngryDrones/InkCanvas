using ClosedXML.Excel;
using InkCanvas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _1.Controllers
{
    [Authorize(Roles = "user,admin")]
    public class ServicesController : Controller
    {
        private readonly CloneIdentityContext _context;
        private readonly UserManager<User> _userManager;

        public ServicesController(CloneIdentityContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult UserServices()
        {
            return View("./Views/Services/UserServices.cshtml");
        }

        public IActionResult AdminServices()
        {
            return View("./Views/Services/AdminServices.cshtml");
        }

        public async Task<IActionResult> Import(IFormFile file)
        {
            List<string> errorMessages = new List<string>();

            if (file == null || file.Length == 0)
            {
                errorMessages.Add("Щось не так із файлом, спробуйте ще раз");
                TempData["ErrorMessages"] = errorMessages.ToArray();
                return RedirectToAction("UserServices");
            }

            try
            {
                if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
                {
                    errorMessages.Add("Дозволені лише файли з розширенням .xlsx та allowed.");
                    TempData["ErrorMessages"] = errorMessages.ToArray();
                    return RedirectToAction("UserServices");
                }

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rows = worksheet.RowsUsed();

                        int rowNum = 2;
                        foreach (var row in rows.Skip(1))
                        {
                            var currentUserId = _userManager.GetUserId(User);

                            var caption = row.Cell(1).Value.ToString();
                            var description = row.Cell(2).Value.ToString();
                            var imageUrl = row.Cell(3).Value.ToString();

                            var userExists = await _userManager.FindByIdAsync(currentUserId) != null;
                            if (!userExists)
                            {
                                errorMessages.Add("Користувача з таким ID не знайдено.");
                                break;
                            }

                            if (caption.Length > 64)
                            {
                                errorMessages.Add($"Рядок {rowNum}: Назва публікації перевищує 64 символи.");
                                continue;
                            }
                            if (caption.Length == 0)
                            {
                                errorMessages.Add($"Рядок {rowNum}: Назва публікації не може бути пустою.");
                                continue;
                            }

                            if (description.Length > 254)
                            {
                                errorMessages.Add($"Рядок {rowNum}: Опис перевищує 254 символи.");
                                continue;
                            }

                            if (imageUrl.Length > 254)
                            {
                                errorMessages.Add($"Рядок {rowNum}: Посилання на фото перевищує 254 символи.");
                                continue;
                            }

                            var post = new Post
                            {
                                UserId = currentUserId,
                                Caption = caption,
                                Description = description,
                                ImageUrl = imageUrl,
                                Date = DateTime.Now
                            };

                            _context.Posts.Add(post);

                            rowNum++;
                        }

                        await _context.SaveChangesAsync();
                    }
                }

                TempData["ErrorMessages"] = errorMessages.ToArray();
                return RedirectToAction("UserServices");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error importing posts: {ex.Message}");
                return RedirectToAction("UserServices");
            }
        }

        // Logged in user export
        public async Task<IActionResult> Export()
        {
            var currentUserId = _userManager.GetUserId(User);

            var exportPosts = await _context.Posts
                .Where(post => post.UserId == currentUserId)
                .ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Posts");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "Caption";
                worksheet.Cell(currentRow, 2).Value = "Description";
                worksheet.Cell(currentRow, 3).Value = "ImageUrl";
                #endregion

                #region Body
                foreach (var post in exportPosts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = post.Caption;
                    worksheet.Cell(currentRow, 2).Value = post.Description;
                    worksheet.Cell(currentRow, 3).Value = post.ImageUrl;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "UserPosts.xlsx");
                }
            }
        }

        // All posts export
        [Authorize(Roles="admin")]
        public IActionResult ExportAll()
        {
            var exportPosts = _context.Posts.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Posts");
                var currentRow = 1;

                #region Header
                worksheet.Cell(currentRow, 1).Value = "UserId";
                worksheet.Cell(currentRow, 2).Value = "Caption";
                worksheet.Cell(currentRow, 3).Value = "Description";
                worksheet.Cell(currentRow, 4).Value = "ImageUrl";
                #endregion

                #region Body
                foreach (var post in exportPosts)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = post.UserId;
                    worksheet.Cell(currentRow, 2).Value = post.Caption;
                    worksheet.Cell(currentRow, 3).Value = post.Description;
                    worksheet.Cell(currentRow, 4).Value = post.ImageUrl;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "AllPosts.xlsx");
                }
            }
        }
    }
}
