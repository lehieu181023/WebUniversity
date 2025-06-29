﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly DBContext _db;
        private const string KeyCache = "Role";

        public RoleController(DBContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Role,Role.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Role,Role.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Role> listData = null;
            try
            {
                var list = _db.Role.AsQueryable();


                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();

            }
            catch (Exception ex)
            {
                
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Role,Role.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Role objData = await _db.Role.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }

            return PartialView(objData);
        }

        [Authorize(Roles = "Role,Role.Create")]
        public PartialViewResult Create()
        {
            var lstRole = _db.Role.OrderByDescending(x => x.CreateDay).ToList();
            ViewData["lstRole"] = lstRole;
            return PartialView();
        }

        [Authorize(Roles = "Role,Role.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,RoleCode,ParentId,Status")] Models.Role obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Role.Add(obj);
                    await _db.SaveChangesAsync();
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

        [Authorize(Roles = "Role,Role.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Role obj = await _db.Role.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var lstRole = _db.Role.Where(m => m.Id != id).ToList();
            ViewData["lstRole"] = lstRole;
            return PartialView(obj);
        }

        [Authorize(Roles = "Role,Role.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,RoleCode,ParentId,Status")] Models.Role obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Role.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.RoleCode = obj.RoleCode;

                objData.ParentId = obj.ParentId;
                
                objData.Status = obj.Status;

                await _db.SaveChangesAsync();
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

        [Authorize(Roles = "Role,Role.Delete")]
        public JsonResult Delete(int? id)
        {
            Role obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Role.Find(id);
                if (obj != null)
                {
                    _db.Role.Remove(obj);
                    _db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Role,Role.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Role.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
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
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}