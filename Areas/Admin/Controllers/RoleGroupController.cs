
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
        private readonly DBContext db = new DBContext();
        private const string KeyCache = "RoleGroup";
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.RoleGroup> listData = null;
            try
            {
                var list = db.RoleGroup.AsQueryable();

                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();

            }
            catch (Exception ex)
            {
                
            }
            return PartialView(listData);
        }

        public async Task<ActionResult> RoleInRoleGroup(int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.RoleGroup obj = await db.RoleGroup.FindAsync(Id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            var lstRole = db.Role.Where(x => x.Status).ToList();
            ViewData["lstRole"] = lstRole;
            var lstRoleInRoleGroup = db.RoleInRoleGroup.Where(x => x.RoleGroupId == Id).Select(m => m.RoleId).ToHashSet();
            // Chuyển lstRoleInRoleGroup thành HashSet để tối ưu tìm kiếm
            var roleIdSet = lstRoleInRoleGroup as HashSet<int>;

            ViewData["lstRoleInRoleGroup"] = lstRoleInRoleGroup;
            return PartialView(obj);
        }

        public PartialViewResult Create()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<JsonResult> EditRoleInRoleGroup(int Id = 0, string selectedValues = "")
        {
            try
            {
                // Lấy danh sách RoleId đã có trong nhóm
                var existingRoles = db.RoleInRoleGroup.Where(x => x.RoleGroupId == Id).ToList();

                if (string.IsNullOrWhiteSpace(selectedValues))
                {
                    // Nếu không có giá trị nào được chọn, xóa hết quyền của nhóm
                    db.RoleInRoleGroup.RemoveRange(existingRoles);
                }
                else
                {
                    // Chuyển chuỗi thành danh sách các RoleId
                    var newRoleIds = selectedValues
                                     .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                     .Select(v => int.Parse(v.Trim())) // Chuyển về int
                                     .ToHashSet(); // Dùng HashSet để tối ưu tìm kiếm

                    // Xóa các quyền không còn trong danh sách mới
                    var rolesToRemove = existingRoles.Where(x => !newRoleIds.Contains(x.RoleId)).ToList();
                    db.RoleInRoleGroup.RemoveRange(rolesToRemove);

                    // Thêm các quyền mới chưa có trong database
                    var existingRoleIds = existingRoles.Select(x => x.RoleId).ToHashSet();
                    var rolesToAdd = newRoleIds.Where(x => !existingRoleIds.Contains(x))
                                               .Select(roleId => new RoleInRoleGroup() { RoleId = roleId, RoleGroupId = Id })
                                               .ToList();
                    db.RoleInRoleGroup.AddRange(rolesToAdd);
                }

                await db.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Cập nhật thất bại, vui lòng thử lại!" });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,NameRoleGroup,Status")] Models.RoleGroup obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.RoleGroup.Add(obj);
                    await db.SaveChangesAsync();
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Thêm mới thành công" });
        }

        
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.RoleGroup obj = await db.RoleGroup.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            return PartialView(obj);
        }
        
        [HttpPost]

        public async Task<JsonResult> EditPost([Bind("Id,NameRoleGroup,Status")] Models.RoleGroup obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await db.RoleGroup.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.NameRoleGroup = obj.NameRoleGroup;

                objData.Status = obj.Status;

                await db.SaveChangesAsync();
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
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        
        public JsonResult Delete(int? id)
        {
            RoleGroup obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = db.RoleGroup.Find(id);
                if (obj != null)
                {
                    db.RoleInRoleGroup.RemoveRange(db.RoleInRoleGroup.Where(x => x.RoleGroupId == id));
                    db.RoleGroup.Remove(obj);
                    db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await db.RoleGroup.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {

                return Json(new { success = false, message = "Không thay đổi được trạng thái bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được cập nhật trạng thái thành công" });
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