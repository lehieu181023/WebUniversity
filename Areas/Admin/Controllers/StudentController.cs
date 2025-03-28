
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Security.Claims;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentController : Controller
    {
        private readonly DBContext db = new DBContext();
        private const string KeyCache = "Student";

        [Authorize(Roles = "Student|Student.View")]
        public ActionResult Index(int ClassId = 0)
       {
            var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
            if (ClassId > 0)
            {
                ViewBag.ReturnClass = true;
            }
            return View();
        }

        [Authorize(Roles = "Student|Student.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData(int ClassId = 0)
        {
            List<Models.Student> listData = null;
            try
            {
                var list = db.Student.AsQueryable();
                list = list.Include(x => x.Class);
                if (ClassId > 0)
                {
                    list = list.Where(x => x.ClassId == ClassId);
                }

                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                
            }
            
            return PartialView(listData);
        }

        [Authorize(Roles = "Student|Student.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Student objData = await db.Student.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }
            var listclass = db.Class.ToList();
            ViewData["listclass"] = listclass;
            return PartialView(objData);
        }

        [Authorize(Roles = "Student|Student.Create")]
        public PartialViewResult Create()
        {
            var listclass = db.Class.ToList();
            ViewData["listclass"] = listclass;
            return PartialView();
        }

        [Authorize(Roles = "Student|Student.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,FullName,BirthDate,Gender,Address,Email,PhoneNumber,ClassId,Status,Image,Cccd")] Models.Student obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Student.Add(obj);
                    await db.SaveChangesAsync();
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new { success = false, message = "Lỗi dữ liệu nhập: " + string.Join("; ", errors) });
                }

            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        return Json(new { success = false, message = "Email hoặc Số điện thoại đã tồn tại!" });
                    }
                }

                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Thêm mới thành công" });
        }


        [Authorize(Roles = "Student|Student.Create")]
        public PartialViewResult CreateAccount(string selectedValues = "")
        {
            ViewData["selectedValues"] = selectedValues;
            return PartialView();
        }

        [Authorize(Roles = "Student|Student.Create")]
        [HttpPost]
        public async Task<JsonResult> CreateAcountStdent(string selectedValues = "", string Password = "")
        {
            try
            {
                // Chuyển chuỗi thành danh sách các RoleId
                var newRoleIds = selectedValues
                                 .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(v => int.Parse(v.Trim())) // Chuyển về int
                                 .ToHashSet(); // Dùng HashSet để tối ưu tìm kiếm
                List<Account> lstAccount = new List<Account>();
                Password = new PasswordHelper().HashPassword(Password);
                foreach (var item in newRoleIds)
                {
                    var student = db.Student.Find(item);
                    if (student != null)
                    {
                        Account obj = new Account
                        {
                            Username = student.StudentCode,
                            Password = Password,
                            RoleGroupId = 2010,
                            StudentId = item,
                            Status = true
                        };
                        lstAccount.Add(obj);
                    }
                }

                if (lstAccount.Count > 0)
                {
                    db.Account.AddRange(lstAccount);
                    await db.SaveChangesAsync(); // Lưu vào database
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        return Json(new { success = false, message = "Có học sinh đã tạo tài khoản!" });
                    }
                }
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Thêm tài khoản thành công" });
        }

        [Authorize(Roles = "Student|Student.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Student obj = await db.Student.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var listclass = db.Class.ToList();
            ViewData["listclass"] = listclass;
            return PartialView(obj);
        }

        [Authorize(Roles = "Student|Student.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,FullName,BirthDate,Gender,Address,Email,PhoneNumber,ClassId,Status,Image,Cccd")] Models.Student obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await db.Student.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                objData.FullName = obj.FullName;
                objData.BirthDate = obj.BirthDate;
                objData.Gender = obj.Gender;
                objData.Address = obj.Address;
                objData.Email = obj.Email;
                objData.PhoneNumber = obj.PhoneNumber;
                objData.ClassId = obj.ClassId;
                objData.Status = obj.Status;
                objData.Image = obj.Image;
                objData.Cccd = obj.Cccd;
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

        [Authorize(Roles = "Student|Student.Delete")]
        public JsonResult Delete(int? id)
        {
            Student obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = db.Student.Find(id);
                if (obj != null)
                {
                    db.Student.Remove(obj);
                    db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Student|Student.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await db.Student.FindAsync(id);
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