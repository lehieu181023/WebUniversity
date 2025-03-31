
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<RoomController> _logger;
        private const string KeyCache = "Room";

        public RoomController(DBContext db, ILogger<RoomController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Room|Room.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Room> listData = null;
            try
            {
                var list = _db.Room.AsNoTracking().AsQueryable();
                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching room list data");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Room|Room.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Room objData = await _db.Room.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(objData);
        }

        [Authorize(Roles = "Room|Room.Create")]
        public PartialViewResult Create()
        {
            return PartialView();
        }

        [Authorize(Roles = "Room|Room.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,Name,Building,Floor,Vacuity,Status")] Models.Room obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ: {JsonConvert.SerializeObject(obj)}");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }

                _db.Room.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo phòng mới: Name = {obj.Name}, Building = {obj.Building}");
                return Json(new { success = true, message = "Thêm mới thành công" });
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";
                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm phòng: {JsonConvert.SerializeObject(obj)}");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "Room|Room.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Room obj = await _db.Room.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(obj);
        }

        [Authorize(Roles = "Room|Room.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,Name,Building,Floor,Vacuity,Status")] Models.Room obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Room.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.Name = obj.Name;
                objData.Building = obj.Building;
                objData.Floor = obj.Floor;
                objData.Vacuity = obj.Vacuity;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật phòng: Id = {objData.Id}, Name = {objData.Name}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật phòng: {JsonConvert.SerializeObject(obj)}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        [Authorize(Roles = "Room|Room.Delete")]
        public JsonResult Delete(int? id)
        {
            Room obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Room.Find(id);
                if (obj != null)
                {
                    _db.Room.Remove(obj);
                    _db.SaveChanges();

                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa phòng: Id = {obj.Id}, Name = {obj.Name}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa phòng: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Room|Room.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Room.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật trạng thái phòng: Id = {objData.Id}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật trạng thái phòng: Id = {id}");
                return Json(new { success = false, message = "Không thay đổi được trạng thái bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
        }

        [Authorize(Roles = "Room|Room.Edit")]
        public async Task<JsonResult> Vacuity(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Room.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Vacuity = !objData.Vacuity;
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật trạng thái phòng: Id = {objData.Id}, Vacuity = {objData.Vacuity}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật trạng thái phòng: Id = {id}");
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