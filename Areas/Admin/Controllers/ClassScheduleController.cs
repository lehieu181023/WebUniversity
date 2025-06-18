
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassScheduleController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ClassScheduleController> _logger;
        private const string KeyCache = "ClassSchedule";

        public ClassScheduleController(DBContext db, ILogger<ClassScheduleController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            try
            {
                var listData = await _db.ClassSchedule
                    .Include(x => x.Class)
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Lecturer)
                    .Include(x => x.Room)
                    .Include(x => x.ClassShift)
                    .OrderByDescending(g => g.CreateDay)
                    .ToListAsync();

                return PartialView("ListData", listData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return PartialView("_ErrorPartial");
            }
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.ClassSchedule objData = await _db.ClassSchedule
                    .Include(x => x.Class)
                    .Include(x => x.Course)
                        .ThenInclude(x => x.Lecturer)
                    .Include(x => x.Room)
                    .Include(x => x.ClassShift)
                    .SingleOrDefaultAsync(x => x.Id == id);

            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }
            return PartialView(objData);
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.Create")]
        public PartialViewResult Create()
        {
            var lstCourse = _db.Course.Where(s => s.Status).ToList();
            var lstCS = _db.ClassShift.Where(s => s.Status).ToList();
            var lstRoom = _db.Room.Where(s => s.Status).ToList();
            var lstClass = _db.Class.Where(s => s.Status).ToList();

            ViewData["lstCourse"] = new SelectList(lstCourse, "Id", "CourseName");
            ViewData["lstCS"] = new SelectList(lstCS, "Id", "Name");
            ViewData["lstRoom"] = new SelectList(lstRoom, "Id", "Name");
            ViewData["lstClass"] = new SelectList(lstClass, "Id", "ClassName");

            return PartialView();
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,CourseId,ClassId,ClassShiftId,RoomId,StartDay,EndDay,DayOfWeek,Status")] Models.ClassSchedule obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
                if(obj.StartDay > obj.EndDay)
                {
                    return Json(new { success = false, message = "Ngày bắt đầu không được lớn hơn ngày kết thúc" });
                }
                _db.ClassSchedule.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo lịch học mới: ClassId = {obj.ClassId}, CourseId = {obj.CourseId}");
                return Json(new { success = true, message = "Thêm mới thành công" });
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 50000)
                    {
                        return Json(new { success = false, message = sqlException.Message });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm lịch học");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.ClassSchedule obj = await _db.ClassSchedule.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var lstCourse = _db.Course.Where(s => s.Status).ToList();
            var lstCS = _db.ClassShift.Where(s => s.Status).ToList();
            var lstRoom = _db.Room.Where(s => s.Status).ToList();
            var lstClass = _db.Class.Where(s => s.Status).ToList();

            ViewData["lstCourse"] = new SelectList(lstCourse, "Id", "CourseName");
            ViewData["lstCS"] = new SelectList(lstCS, "Id", "Name");
            ViewData["lstRoom"] = new SelectList(lstRoom, "Id", "Name");
            ViewData["lstClass"] = new SelectList(lstClass, "Id", "ClassName");
            return PartialView(obj);
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,CourseId,ClassId,ClassShiftId,RoomId,StartDay,EndDay,DayOfWeek,Status")] Models.ClassSchedule obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.ClassSchedule.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.CourseId = obj.CourseId;
                objData.ClassId = obj.ClassId;
                objData.ClassShiftId = obj.ClassShiftId;
                objData.RoomId = obj.RoomId;
                objData.StartDay = obj.StartDay;
                objData.EndDay = obj.EndDay;
                objData.DayOfWeek = obj.DayOfWeek;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật lịch học: Id = {objData.Id}");
                return Json(new { success = true, message = "Cập nhật thành công" });
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
                if (ex.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 50000)
                    {
                        return Json(new { success = false, message = sqlException.Message });
                    }
                }
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật lịch học");
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.Delete")]
        public JsonResult Delete(int? id)
        {
            ClassSchedule obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.ClassSchedule.Find(id);
                if (obj != null)
                {
                    _db.ClassSchedule.Remove(obj);
                    _db.SaveChanges();

                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa lịch học: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa lịch học: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "ClassSchedule,ClassSchedule.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.ClassSchedule.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái lịch học: Id = {objData.Id}, Status = {objData.Status}");
                return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái lịch học: Id = {id}");
                return Json(new { success = false, message = "Không thay đổi được trạng thái bản ghi này" });
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