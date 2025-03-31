
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
    public class AccountController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<Account> _logger;
        private const string KeyCache = "Account";

        public AccountController(DBContext db, ILogger<Account> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize (Roles = "Account")]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Account,Account.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Account> listData = null;
            try
            {
                var list = _db.Account.AsQueryable();
                list = list.Include(x => x.RoleGroup);

                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();

            }
            catch (Exception ex)
            {
                
            }
            return PartialView(listData);
        }
        [Authorize(Roles = "Account,Account.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Account objData = await _db.Account.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }
            var listfaculty = _db.Account.ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(objData);
        }
        [Authorize(Roles = "Account,Account.Create")]
        public PartialViewResult Create()
        {
            var lstRoleGR = _db.RoleGroup.Where(m => m.Status).ToList();
            ViewData["lstRoleGR"] = lstRoleGR;
            return PartialView();
        }
        [Authorize(Roles = "Account,Account.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,Username,Password,RoleGroupId,StudentId,LecturerId,Status")] Models.Account obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }

                obj.Password = new PasswordHelper().HashPassword(obj.Password);
                _db.Account.Add(obj);
                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo tài khoản mới: Username = {obj.Username}, RoleGroupId = {obj.RoleGroupId}");
                return Json(new { success = true, message = "Thêm mới thành công" });
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm tài khoản thất bại: Username {obj.Username} đã tồn tại.");
                        return Json(new { success = false, message = "Username đã tồn tại!" });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm tài khoản");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }




        [Authorize(Roles = "Account,Account.Edit")]
        public async Task<ActionResult> Edit(int? id)
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

            var lstRoleGR = _db.RoleGroup.Where(m => m.Status).ToList();
            ViewData["lstRoleGR"] = lstRoleGR;
            return PartialView(obj);
        }
        [Authorize(Roles = "Account,Account.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,RoleGroupId,Status")] Models.Account obj, int? Id)
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

            try
            {
                objData.RoleGroupId = obj.RoleGroupId;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật tài khoản: Id = {objData.Id}, RoleGroupId = {objData.RoleGroupId}, Status = {objData.Status}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật tài khoản: {obj.Username}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }
        [Authorize(Roles = "Account,Account.Edit")]
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
        [Authorize(Roles = "Account,Account.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPasswordPost([Bind("Id,Password")] Models.Account obj, int? Id)
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

            try
            {
                objData.Password = new PasswordHelper().HashPassword(obj.Password);

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật mật khẩu tài khoản: Id = {objData.Id}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật mật khẩu tài khoản: {obj.Username}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Đổi mật khẩu thành công" });
        }
        [Authorize(Roles = "Account,Account.Delete")]
        public JsonResult Delete(int? id)
        {
            Account obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Account.Find(id);
                if (obj != null)
                {
                    _db.Account.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa tài khoản: Id = {obj.Id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa tài khoản: Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Account,Account.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Account.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật trạng thái tài khoản: Id = {objData.Id}, Status = {objData.Status}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật trạng thái tài khoản: Id = {id}");
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