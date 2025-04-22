
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LecturerController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<LecturerController> _logger;
        private const string KeyCache = "Lecturer";

        public LecturerController(DBContext db, ILogger<LecturerController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Roles = "Lecturer,Lecturer.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Lecturer,Lecturer.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Lecturer> listData = null;
            try
            {
                var list = _db.Lecturer.AsNoTracking().AsQueryable();
                listData = await list.Include(x => x.Faculty).OrderByDescending(g => g.CreateDay).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lecturer list data.");
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Lecturer,Lecturer.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Lecturer objData = await _db.Lecturer.Include(x=>x.Faculty).AsNoTracking().FirstOrDefaultAsync(l => l.Id == id); ;
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }
            return PartialView(objData);
        }

        [Authorize(Roles = "Lecturer,Lecturer.Create")]
        public PartialViewResult Create()
        {
            var listfaculty = _db.Faculty.AsNoTracking().ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView();
        }

        [Authorize(Roles = "Lecturer,Lecturer.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,FullName,BirthDate,Gender,Address,Email,PhoneNumber,FacultyId,Status,Image,Cccd")] Models.Lecturer obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Lecturer.Add(obj);
                    await _db.SaveChangesAsync();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo giảng viên mới: {obj.FullName} - {obj.LecturerCode}");
                }
                else
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    _logger.LogWarning($"[{User.Identity?.Name}] Nhập dữ liệu không hợp lệ");
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
                        _logger.LogWarning($"[{currentUser}] Thêm giảng viên thất bại: Email hoặc Số điện thoại {obj.Email} đã tồn tại.");
                        return Json(new { success = false, message = "Email hoặc Số điện thoại đã tồn tại!" });
                    }
                }

                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm giảng viên");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Thêm mới thành công" });
        }

        [Authorize(Roles = "Lecturer,Lecturer.Create")]
        public PartialViewResult CreateAccount(string selectedValues = "")
        {
            var lstRoleGR = _db.RoleGroup.AsNoTracking().Where(m => m.Status).ToList();
            ViewData["selectedValues"] = selectedValues;
            return PartialView();
        }

        [Authorize(Roles = "Lecturer,Lecturer.Create")]
        [HttpPost]
        public async Task<JsonResult> CreateAcountLecturer(string selectedValues = "", string Password = "")
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
                    var lecturer = _db.Lecturer.Find(item);
                    if (lecturer != null)
                    {
                        Account obj = new Account
                        {
                            Username = lecturer.LecturerCode,
                            Password = Password,
                            RoleGroupId = 5,
                            LecturerId = item,
                            Status = true
                        };
                        lstAccount.Add(obj);
                    }
                }

                if (lstAccount.Count > 0)
                {
                    _db.Account.AddRange(lstAccount);
                    await _db.SaveChangesAsync(); // Lưu vào database
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã tạo tài khoản giảng viên mới");
                }
            }
            catch (Exception ex)
            {
                string currentUser = User.Identity?.Name ?? "Unknown";

                if (ex.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Lỗi UNIQUE constraint
                    {
                        _logger.LogWarning($"[{currentUser}] Thêm tài khoản giảng viên thất bại: Có học sinh đã tạo tài khoản.");
                        return Json(new { success = false, message = "Có giảng viên đã được tạo tài khoản!" });
                    }
                }
                _logger.LogError(ex, $"[{currentUser}] Lỗi khi thêm tài khoản giảng viên");
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
            return Json(new { success = true, message = "Thêm tài khoản thành công" });
        }

        [Authorize(Roles = "Lecturer,Lecturer.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Lecturer obj = await _db.Lecturer.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id); ;
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var listfaculty = _db.Faculty.AsNoTracking().ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(obj);
        }

        [Authorize(Roles = "Lecturer,Lecturer.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,FullName,BirthDate,Gender,Address,Email,PhoneNumber,FacultyId,Status,Image,Cccd")] Models.Lecturer obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Lecturer.FindAsync(Id);
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
                objData.FacultyId = obj.FacultyId;
                objData.Status = obj.Status;
                objData.Image = obj.Image;
                objData.Cccd = obj.Cccd;

                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật giảng viên: {obj.FullName}-{obj.LecturerCode}");
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật giảng viên");
                return Json(new { success = false, message = "Không thể lưu được" });
            }

            return Json(new { success = true, message = "Cập nhật thành công" });
        }

        [Authorize(Roles = "Lecturer,Lecturer.Delete")]
        public JsonResult Delete(int? id)
        {
            Lecturer obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Lecturer.Find(id);
                if (obj != null)
                {
                    _db.Lecturer.Remove(obj);
                    _db.SaveChanges();
                    _logger.LogInformation($"[{User.Identity?.Name}] Đã xóa giảng viên: {obj.FullName} - {obj.LecturerCode}");
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi xóa giảng viên: {id}");
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Lecturer,Lecturer.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Lecturer.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Bản ghi đã bị xóa" });
            }
            try
            {
                objData.Status = !objData.Status;
                await _db.SaveChangesAsync();
                _logger.LogInformation($"[{User.Identity?.Name}] Đã cập nhật trạng thái giảng viên: {objData.FullName} - {objData.LecturerCode}");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi cập nhật trạng thái giảng viên: {id}");
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