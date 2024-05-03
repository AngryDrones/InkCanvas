using ClosedXML.Excel;
using InkCanvas.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace _1.Controllers
{
    public class ServicesController : Controller
    {

        private readonly CloneIdentityContext _context;

        public ServicesController(CloneIdentityContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View("./Views/Services/Index.cshtml");
        }

        private readonly UserManager<User> _userManager;

        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a file.");
                return RedirectToAction("Index"); // Redirect to a suitable action
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed();

                    foreach (var row in rows.Skip(1)) // Skip header row
                    {
                        var user = new User
                        {
                            Login = row.Cell(4).Value.ToString(), // Assuming Login is in the 4th column
                            UserName = row.Cell(2).Value.ToString(), // Assuming UserName is in the 2nd column
                            Email = row.Cell(2).Value.ToString(), // Assuming Email is in the 2nd column
                            Age = int.Parse(row.Cell(5).Value.ToString()), // Assuming Age is in the 5th column
                            PasswordHash = row.Cell(6).Value.ToString()
                        };

                        var result = await _userManager.CreateAsync(user);
                        if (result.Succeeded)
                        {
                            await _userManager.AddToRoleAsync(user, "user");
                        }
                        else
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("Index"); // Redirect to a suitable action
        }

        public IActionResult Export()
        {
            var exportUsers = _context.Users.ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Users");
                var currentRow = 1;
                #region Header
                worksheet.Cell(currentRow, 1).Value = "UserId";
                worksheet.Cell(currentRow, 2).Value = "Email";
                worksheet.Cell(currentRow, 3).Value = "PhoneNumber";
                worksheet.Cell(currentRow, 4).Value = "Login";
                worksheet.Cell(currentRow, 5).Value = "Age";
                worksheet.Cell(currentRow, 6).Value = "PasswordHash";
                #endregion

                #region Body
                foreach (var user in exportUsers)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = user.Id;
                    worksheet.Cell(currentRow, 2).Value = user.Email;
                    worksheet.Cell(currentRow, 3).Value = user.PhoneNumber;
                    worksheet.Cell(currentRow, 4).Value = user.Login;
                    worksheet.Cell(currentRow, 5).Value = user.Age;
                    worksheet.Cell(currentRow, 6).Value = user.PasswordHash;
                }
                #endregion

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "Users.xlsx");
                }
            }
        }
    }
}
