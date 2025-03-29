using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebUniversity.Models;

namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        private readonly DBContext _db;

        public DashBoardController(DBContext db)
        {
            _db = db;
        }

        [Authorize(Policy = "NotLectureOrStudent")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> report1()
        {
            var currentYear = DateTime.UtcNow.Year;
            var startYear = currentYear - 4; // Lấy dữ liệu từ 5 năm gần nhất

            // Danh sách các năm cần có dữ liệu
            var years = Enumerable.Range(startYear, 5).ToList();

            // Truy vấn dữ liệu sinh viên
            var studentData = await _db.Student
                .Where(s => s.CreateDay != null && s.CreateDay.Year >= startYear)
                .GroupBy(s => s.CreateDay.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            // Truy vấn dữ liệu giảng viên
            var lecturerData = await _db.Lecturer
                .Where(l => l.CreateDay != null && l.CreateDay.Year >= startYear)
                .GroupBy(l => l.CreateDay.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            // Truy vấn dữ liệu phòng học
            var roomData = await _db.Room
                .Where(r => r.CreateDay != null && r.CreateDay.Year >= startYear)
                .GroupBy(r => r.CreateDay.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            // Đảm bảo dữ liệu luôn có đủ 5 năm
            var studentsFinal = years.Select(y => studentData.FirstOrDefault(d => d.Year == y)?.Count ?? 0).ToList();
            var lecturersFinal = years.Select(y => lecturerData.FirstOrDefault(d => d.Year == y)?.Count ?? 0).ToList();
            var roomsFinal = years.Select(y => roomData.FirstOrDefault(d => d.Year == y)?.Count ?? 0).ToList();


            var data = new
            {
                years = years,
                students = studentsFinal,
                lecturers = lecturersFinal,
                rooms = roomsFinal
            };

            return Json(new { success = true, data });
        }

        public async Task<IActionResult> AccountTB()
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

            var user = await _db.Account.Include(x => x.Lecturer).Include(x => x.Student).FirstOrDefaultAsync(u => u.Username == username);
            return PartialView(user);
        }

        [Authorize]
        public async Task<ActionResult> EditPassword(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Account obj = _db.Account.Find(id) ?? new Account();
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(obj);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> EditPasswordPost([Bind("Id,Password")] Models.Account obj, int? Id,string PasswordAgain = "" , string PasswordCurent = "")
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Account.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            if (!PasswordAgain.Equals(obj.Password))
            {
                return Json(new { success = false, message = "Mật khẩu nhập lại không giống" });
            }
            if (string.IsNullOrEmpty(PasswordCurent) || !new PasswordHelper().VerifyPassword(objData.Password, PasswordCurent))
            {
                return Json(new { success = false, message = "Mật khẩu không chính xác" });
            }

            try
            {
                objData.Password = new PasswordHelper().HashPassword(obj.Password);

                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    return Json(new { success = false, message = "Bản ghi này đã bị xóa bởi người dùng khác" });
                }
                else
                {
                    return Json(new { success = false, message = "Bản ghi này đã bị sửa bởi người dùng khác" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Đổi mật khẩu thành công" });
        }

    }
}
