
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Linq;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleGroupController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<RoleGroupController> _logger;
        private const string KeyCache = "RoleGroup";

        public RoleGroupController(DBContext db, ILogger<RoleGroupController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.RoleGroup> listData = null;
            try
            {
                var list = _db.RoleGroup.AsNoTracking().AsQueryable();
                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching RoleGroup list data.");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.View")]
        public async Task<ActionResult> RoleInRoleGroup(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.RoleGroup obj = await _db.RoleGroup.AsNoTracking().FirstOrDefaultAsync(rg => rg.Id == Id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            var lstRole = _db.Role.AsNoTracking().Where(x => x.Status).ToList();
            ViewData["lstRole"] = lstRole;
            var lstRoleInRoleGroup = _db.RoleInRoleGroup.AsNoTracking().Where(x => x.RoleGroupId == Id).Select(m => m.RoleId).ToHashSet();
            var roleIdSet = lstRoleInRoleGroup as HashSet<int>;

            ViewData["lstRoleInRoleGroup"] = lstRoleInRoleGroup;
            return PartialView(obj);
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Create")]
        public PartialViewResult Create()
        {
            return PartialView();
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditRoleInRoleGroup(int Id = 0, string selectedValues = "")
        {
            try
            {
                var existingRoles = _db.RoleInRoleGroup.Where(x => x.RoleGroupId == Id).ToList();

                if (string.IsNullOrWhiteSpace(selectedValues))
                {
                    _db.RoleInRoleGroup.RemoveRange(existingRoles);
                }
                else
                {
                    var newRoleIds = selectedValues
                                     .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(v => int.Parse(v.Trim()))
                                     .ToHashSet();

                    var rolesToRemove = existingRoles.Where(x => !newRoleIds.Contains(x.RoleId)).ToList();
                    _db.RoleInRoleGroup.RemoveRange(rolesToRemove);

                    var existingRoleIds = existingRoles.Select(x => x.RoleId).ToHashSet();
                    var rolesToAdd = newRoleIds.Where(x => !existingRoleIds.Contains(x))
                                               .Select(roleId => new RoleInRoleGroup() { RoleId = roleId, RoleGroupId = Id })
                                               .ToList();
                    _db.RoleInRoleGroup.AddRange(rolesToAdd);
                }

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Updated roles in RoleGroup Id = {Id}");
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Error updating roles in RoleGroup Id = {Id}");
                return Json(new { success = false, message = "Cập nhật thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,NameRoleGroup,Status")] Models.RoleGroup obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.RoleGroup.Add(obj);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"[{User.Identity?.Name}] Created new RoleGroup: {JsonConvert.SerializeObject(obj)}");
                    return Json(new { success = true, message = "Thêm mới thành công" });
                }
                else
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Invalid data input: {JsonConvert.SerializeObject(obj)}");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Error creating RoleGroup: {JsonConvert.SerializeObject(obj)}");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.RoleGroup obj = await _db.RoleGroup.AsNoTracking().FirstOrDefaultAsync(rg => rg.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            return PartialView(obj);
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,NameRoleGroup,Status")] Models.RoleGroup obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.RoleGroup.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.NameRoleGroup = obj.NameRoleGroup;
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Updated RoleGroup Id = {Id}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Error updating RoleGroup Id = {Id}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Delete")]
        public JsonResult Delete(int? id)
        {
            RoleGroup obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.RoleGroup.Find(id);
                if (obj != null)
                {
                    _db.RoleInRoleGroup.RemoveRange(_db.RoleInRoleGroup.Where(x => x.RoleGroupId == id));
                    _db.RoleGroup.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Deleted RoleGroup Id = {id}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Error deleting RoleGroup Id = {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "RoleGroup|RoleGroup.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.RoleGroup.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Updated status of RoleGroup Id = {id}");
                return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Error updating status of RoleGroup Id = {id}");
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