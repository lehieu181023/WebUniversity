using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WebUniversity.Models;

namespace WebUniversity.Areas.Lec.Controllers
{
    [Area("Lecturer")]
    [Authorize(Roles = "LecturerRole")]
    public class InfoController : Controller
    {
        private readonly DBContext db = new DBContext();
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
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

            var lecturer = db.Lecturer.Include(x => x.Faculty).SingleOrDefault(l => l.LecturerCode == username);
            if (lecturer == null)
            {
                TempData["ErrorMessage"] = "Giảng viên không tồn tại!";
                return RedirectToAction("Login", "Account" ,new { area = "" });
            }

            return View(lecturer);
        }

    }
}
