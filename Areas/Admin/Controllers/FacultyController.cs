
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FacultyController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<FacultyController> _logger;
        private const string KeyCache = "Faculty";

        public FacultyController(DBContext db, ILogger<FacultyController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "Faculty,Faculty.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Faculty,Faculty.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Faculty> listData = null;
            try
            {
                var list = _db.Faculty.AsNoTracking().AsQueryable();
                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching faculty list data.");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Faculty,Faculty.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Faculty objData = await _db.Faculty.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            return PartialView(objData);
        }

        [Authorize(Roles = "Faculty,Faculty.Create")]
        public PartialViewResult Create()
        {
            return PartialView();
        }

        [Authorize(Roles = "Faculty,Faculty.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,FacultyName,Status")] Models.Faculty obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }

                _db.Faculty.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo khoa mới: FacultyName = {obj.FacultyName}");
                return Json(new { success = true, message = "Thêm mới thành công" });
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm khoa thất bại: FacultyName {obj.FacultyName} đã tồn tại.");
                        return Json(new { success = false, message = "FacultyName đã tồn tại!" });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm khoa");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "Faculty,Faculty.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Faculty obj = await _db.Faculty.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(obj);
        }

        [Authorize(Roles = "Faculty,Faculty.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,FacultyName,Status")] Models.Faculty obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Faculty.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.FacultyName = obj.FacultyName;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật khoa: FacultyName = {obj.FacultyName}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật khoa");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        [Authorize(Roles = "Faculty,Faculty.Delete")]
        public JsonResult Delete(int? id)
        {
            Faculty obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Faculty.Find(id);
                if (obj != null)
                {
                    _db.Faculty.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa khoa: FacultyName = {obj.FacultyName}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa khoa: {obj.FacultyName}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Faculty,Faculty.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Faculty.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái khoa: FacultyName = {objData.FacultyName}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái khoa: {objData.FacultyName}");
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