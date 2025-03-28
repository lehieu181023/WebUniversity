
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;


namespace WebUniversity.Areas.Stu.Controllers
{
    [Area("Student")]
    public class ScheduleController : Controller
    {
        private readonly DBContext db = new DBContext();
        private const string KeyCache = "ClassSchedule";

        [Authorize(Roles = "StudentRole")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "StudentRole")]
        [HttpGet]
        public async Task<IActionResult> ListData()
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
            try
            {
                var listData = await db.ClassSchedule
                    .Include(x => x.Class)
                        .ThenInclude(x => x.Students)
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Lecturer)
                    .Include(x => x.Room)
                    .Include(x => x.ClassShift)
                    .OrderByDescending(g => g.EndDay)
                    .Where(x => x.Class!.Students.Any(z => z.StudentCode == username))
                    .ToListAsync();

                return PartialView("ListData", listData); // Đặt tên PartialView cụ thể
            }
            catch (Exception ex)
            {
                // Ghi log lỗi
                Console.WriteLine(ex.Message);
                return PartialView("_ErrorPartial"); // Trả về một PartialView thông báo lỗi
            }
        }

        [Authorize(Roles = "StudentRole")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.ClassSchedule objData = await db.ClassSchedule
                    .Include(x => x.Class)
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Lecturer)
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Subject)
                    .Include(x => x.Room)
                    .Include(x => x.ClassShift)
                    .SingleOrDefaultAsync(x => x.Id == id);

            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }
            return PartialView(objData);
        } 

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}