
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubjectController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<SubjectController> _logger;
        private const string KeyCache = "Subject";

        public SubjectController(DBContext db, ILogger<SubjectController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "Subject,Subject.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Subject,Subject.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Subject> listData = null;
            try
            {
                var list = _db.Subject.AsNoTracking().AsQueryable();
                listData = await list.Include(x => x.Faculty).OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subject list data.");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Subject,Subject.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Subject objData = await _db.Subject.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            var listfaculty = _db.Faculty.AsNoTracking().ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(objData);
        }

        [Authorize(Roles = "Subject,Subject.Create")]
        public PartialViewResult Create()
        {
            var listfaculty = _db.Faculty.AsNoTracking().ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView();
        }

        [Authorize(Roles = "Subject,Subject.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,SubjectName,Credit,FacultyId,Status")] Models.Subject obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }

                _db.Subject.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo môn học mới: SubjectName = {obj.SubjectName}, FacultyId = {obj.FacultyId}");
                return Json(new { success = true, message = "Thêm mới thành công" });
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm môn học thất bại: SubjectName {obj.SubjectName} đã tồn tại.");
                        return Json(new { success = false, message = "SubjectName đã tồn tại!" });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm môn học: ");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "Subject,Subject.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Subject obj = await _db.Subject.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var listfaculty = _db.Faculty.AsNoTracking().ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(obj);
        }

        [Authorize(Roles = "Subject,Subject.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,SubjectName,FacultyId,Credit,Status")] Models.Subject obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Subject.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.SubjectName = obj.SubjectName;
                objData.FacultyId = obj.FacultyId;
                objData.Credit = obj.Credit;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật môn học: SubjectName = {obj.SubjectName}, FacultyId = {obj.FacultyId}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật môn học");
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }

        [Authorize(Roles = "Subject,Subject.Delete")]
        public JsonResult Delete(int? id)
        {
            Subject obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Subject.Find(id);
                if (obj != null)
                {
                    _db.Subject.Remove(obj);
                    _db.SaveChanges();

                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa môn học: SubjectName = {obj.SubjectName}, FacultyId = {obj.FacultyId}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa môn học");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Subject,Subject.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Subject.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái môn học: SubjectName = {objData.SubjectName}, FacultyId = {objData.FacultyId}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái môn học");
                return Json(new { success = false, message = "Không thay đổi được trạng thái bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
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