
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
        private readonly DBContext _db;
        private readonly ILogger<StudentController> _logger;
        private const string KeyCache = "Student";

        public StudentController(DBContext db, ILogger<StudentController> logger)
        {
            _db = db;
            _logger = logger;
        }

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
                var list = _db.Student.AsNoTracking().AsQueryable();
                if (ClassId > 0)
                {
                    list = list.Where(x => x.ClassId == ClassId);
                }

                listData = await list.Include(x => x.Class).OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching student list data.");
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
            Models.Student objData = await _db.Student.Include(x => x.Class).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }
            return PartialView(objData);
        }

        [Authorize(Roles = "Student|Student.Create")]
        public PartialViewResult Create()
        {
            var listclass = _db.Class.AsNoTracking().ToList();
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
                    _db.Student.Add(obj);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo học sinh mới: {JsonConvert.SerializeObject(obj)}");
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ: {JsonConvert.SerializeObject(obj)}");
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập: " + string.Join("; ", errors) });
                }

            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm học sinh thất bại: Email hoặc Số điện thoại {obj.Email} đã tồn tại.");
                        return Json(new { success = false, message = "Email hoặc Số điện thoại đã tồn tại!" });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm học sinh: {JsonConvert.SerializeObject(obj)}");
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
            List<Account> lstAccount = new List<Account>();
            try
            {
                // Chuyển chuỗi thành danh sách các RoleId
                var newRoleIds = selectedValues
                                 .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(v => int.Parse(v.Trim())) // Chuyển về int
                                 .ToHashSet(); // Dùng HashSet để tối ưu tìm kiếm
                
                Password = new PasswordHelper().HashPassword(Password);
                foreach (var item in newRoleIds)
                {
                    var student = _db.Student.Find(item);
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
                    _db.Account.AddRange(lstAccount);
                    await _db.SaveChangesAsync(); // Lưu vào database
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo tài khoản mới cho học sinh: {JsonConvert.SerializeObject(lstAccount)}");
                }
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm tài khoản thất bại: Có học sinh đã tạo tài khoản.");
                        return Json(new { success = false, message = "Có học sinh đã tạo tài khoản!" });
                    }
                }
                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm tài khoản: {JsonConvert.SerializeObject(lstAccount)}");
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

            Models.Student obj = await _db.Student.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var listclass = _db.Class.AsNoTracking().ToList();
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

            var objData = await _db.Student.FindAsync(Id);
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
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật học sinh: {JsonConvert.SerializeObject(objData)}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseEntry = entry.GetDatabaseValues();
                if (databaseEntry == null)
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Cập nhật thất bại: Bản ghi này đã bị xóa bởi người dùng khác.");
                    return Json(new { success = false, message = "Bản ghi này đã bị xóa bởi người dùng khác" });
                }
                else
                {
                    _logger.LogWarning($"[{User.Identity?.Name}] Cập nhật thất bại: Bản ghi này đã bị sửa bởi người dùng khác.");
                    return Json(new { success = false, message = "Bản ghi này đã bị sửa bởi người dùng khác" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật học sinh: {JsonConvert.SerializeObject(objData)}");
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
                obj = _db.Student.Find(id);
                if (obj != null)
                {
                    _db.Student.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa học sinh: {JsonConvert.SerializeObject(obj)}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa học sinh: {JsonConvert.SerializeObject(obj)}");
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
            var objData = await _db.Student.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật trạng thái học sinh: {JsonConvert.SerializeObject(objData)}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật trạng thái học sinh: {JsonConvert.SerializeObject(objData)}");
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