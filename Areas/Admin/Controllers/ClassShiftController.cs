
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassShiftController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<ClassShiftController> _logger;
        private const string KeyCache = "ClassShift";

        public ClassShiftController(DBContext db, ILogger<ClassShiftController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "ClassShift|ClassShift.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.ClassShift> listData = null;
            try
            {
                var list = _db.ClassShift.AsQueryable();
                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching ClassShift list data.");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "ClassShift|ClassShift.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.ClassShift objData = await _db.ClassShift.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(objData);
        }

        [Authorize(Roles = "ClassShift|ClassShift.Create")]
        public PartialViewResult Create()
        {
            return PartialView();
        }

        [Authorize(Roles = "ClassShift|ClassShift.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,Name,StartTime,EndTime,Status")] Models.ClassShift obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.ClassShift.Add(obj);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo ClassShift mới: Name = {obj.Name}");
                }
                else
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ: {JsonConvert.SerializeObject(obj)}");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thêm ClassShift: {JsonConvert.SerializeObject(obj)}");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Thêm mới thành công" });
        }

        [Authorize(Roles = "ClassShift|ClassShift.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.ClassShift obj = await _db.ClassShift.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(obj);
        }

        [Authorize(Roles = "ClassShift|ClassShift.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,Name,StartTime,EndTime,Status")] Models.ClassShift obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.ClassShift.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.Name = obj.Name;
                objData.StartTime = obj.StartTime;
                objData.EndTime = obj.EndTime;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật ClassShift: Id = {objData.Id}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật ClassShift: {JsonConvert.SerializeObject(obj)}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        [Authorize(Roles = "ClassShift|ClassShift.Delete")]
        public JsonResult Delete(int? id)
        {
            ClassShift obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.ClassShift.Find(id);
                if (obj != null)
                {
                    _db.ClassShift.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa ClassShift: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa ClassShift: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "ClassShift|ClassShift.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.ClassShift.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã thay đổi trạng thái ClassShift: Id = {objData.Id}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi thay đổi trạng thái ClassShift: Id = {id}");
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