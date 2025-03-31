
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ClassController> _logger;
        private const string KeyCache = "Class";

        public ClassController(DBContext db, ILogger<ClassController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "Class,Class.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Class,Class.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Class> listData = null;
            try
            {
                var list = _db.Class.AsQueryable();
                listData = await list.Include(x => x.Faculty).OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching class list data.");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Class,Class.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Class objData = await _db.Class.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            var listfaculty = _db.Faculty.ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(objData);
        }

        [Authorize(Roles = "Class,Class.Create")]
        public PartialViewResult Create()
        {
            var listfaculty = _db.Faculty.ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView();
        }

        [Authorize(Roles = "Class,Class.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,ClassName,FacultyId,Status")] Models.Class obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }

                _db.Class.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo lớp mới: ClassName = {obj.ClassName}, FacultyId = {obj.FacultyId}");
                return Json(new { success = true, message = "Thêm mới thành công" });
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm lớp thất bại: ClassName {obj.ClassName} đã tồn tại.");
                        return Json(new { success = false, message = "ClassName đã tồn tại!" });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm lớp");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "Class,Class.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Class obj = await _db.Class.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var listfaculty = _db.Faculty.ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(obj);
        }

        [Authorize(Roles = "Class,Class.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,ClassName,FacultyId,Status")] Models.Class obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Class.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.ClassName = obj.ClassName;
                objData.FacultyId = obj.FacultyId;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật lớp: Id = {objData.Id}, ClassName = {objData.ClassName}");
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Bản ghi này đã bị xóa bởi người dùng khác: Id = {Id}");
                    return Json(new { success = false, message = "Bản ghi này đã bị xóa bởi người dùng khác" });
                }
                else
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Bản ghi này đã bị sửa bởi người dùng khác: Id = {Id}");
                    return Json(new { success = false, message = "Bản ghi này đã bị sửa bởi người dùng khác" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật lớp: {Id}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }

        [Authorize(Roles = "Class,Class.Delete")]
        public JsonResult Delete(int? id)
        {
            Class obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Class.Find(id);
                if (obj != null)
                {
                    _db.Class.Remove(obj);
                    _db.SaveChanges();

                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa lớp: Id = {id}, ClassName = {obj.ClassName}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Không xóa được bản ghi này: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Class,Class.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Class.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái lớp: Id = {id}, Status = {objData.Status}");
                return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Không thay đổi được trạng thái bản ghi này: Id = {id}");
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