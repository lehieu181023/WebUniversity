using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebUniversity.Models;

namespace WebUniversity.Areas.Stu.Controllers
{
    [Area("Student")]
    public class InfoController : Controller
    {
        private readonly DBContext _db;

        public InfoController(DBContext db)
        {
            _db = db;
        }

        [Authorize (Roles = "StudentRole")]
        public IActionResult Index()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Account", new { area = "" }); // Điều hướng đến trang đăng nhập
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var student = _db.Student.Include(x => x.Class).FirstOrDefault(l => l.StudentCode == username);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Sinh viên không tồn tại!";
                return RedirectToAction("Login", "Account" ,new { area = "" });
            }

            return View(student);
        }

    }
}
