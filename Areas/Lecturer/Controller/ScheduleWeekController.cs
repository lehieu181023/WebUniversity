
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;


namespace WebUniversity.Areas.Lec.Controllers
{
    [Area("Lecturer")]
    public class ScheduleWeekController : Controller
    {
        private readonly DBContext _db;
        private const string KeyCache = "ClassSchedule";

        public ScheduleWeekController(DBContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "LecturerRole")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "LecturerRole")]
        [HttpGet]
        public async Task<IActionResult> ListData(int week = 1)
        {
            List<ClassSchedule> ListData = null;
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
            try
            {
                var listData = _db.ClassSchedule
                    .Include(x => x.Class)
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Subject)
                    .Include(x => x.Room)
                    .Include(x => x.ClassShift)
                    .OrderByDescending(g => g.EndDay)
                    .Where(x => x.Course!.Lecturer!.LecturerCode == username)
                    .AsQueryable();
                // Lấy ngày hiện tại dưới dạng DateOnly
                DateOnly today = DateOnly.FromDateTime(DateTime.Today);

                // Xác định ngày bắt đầu của tuần hiện tại (Thứ Hai)
                DateOnly startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday);

                // Xác định ngày cuối tuần hiện tại (Chủ Nhật)
                DateOnly endOfWeek = startOfWeek.AddDays(6);

                // Xác định tuần sau
                DateOnly startOfNextWeek = startOfWeek.AddDays(7);
                DateOnly endOfNextWeek = endOfWeek.AddDays(7);
                if (week == 1)
                {
                    listData = listData.Where(x => x.StartDay <= endOfWeek && x.EndDay >= startOfWeek);
                }
                else
                {
                    listData = listData.Where(x => x.StartDay <= endOfNextWeek && x.EndDay >= startOfNextWeek);
                }

                ListData = await listData.Where(x => x.Status == true).ToListAsync();

                var lstClShift = _db.ClassShift.Where(x => x.Status).OrderBy(x => x.StartTime).ToList();
                ViewData["ClassShift"] = lstClShift;
                return PartialView("ListData", ListData); // Đặt tên PartialView cụ thể
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine(ex.Message);
                return PartialView("_ErrorPartial"); // Trả về một PartialView thông báo lỗi
            }
        } 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}