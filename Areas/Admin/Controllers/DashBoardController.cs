using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebUniversity.Models;
using Newtonsoft.Json;

namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashBoardController : Controller
    {
        private readonly DBContext _db;
        private readonly ILogger<DashBoardController> _logger;

        public DashBoardController(DBContext db, ILogger<DashBoardController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [Authorize(Policy = "NotLectureOrStudent")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> report1(int yearcount = 2)
        {
            var currentYear = DateTime.UtcNow.Year;
            var startYear = currentYear - (yearcount - 1); // Lấy dữ liệu từ 5 năm gần nhất

            // Danh sách các năm cần có dữ liệu
            var years = Enumerable.Range(startYear, yearcount).ToList();

            // Truy vấn dữ liệu sinh viên
            var studentData = await _db.Student
                .AsNoTracking()
                .Where(s => s.CreateDay != null && s.CreateDay.Year >= startYear)
                .GroupBy(s => s.CreateDay.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            // Truy vấn dữ liệu giảng viên
            var lecturerData = await _db.Lecturer
                .AsNoTracking()
                .Where(l => l.CreateDay != null && l.CreateDay.Year >= startYear)
                .GroupBy(l => l.CreateDay.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            // Truy vấn dữ liệu phòng học
            var roomData = await _db.Room
                .AsNoTracking()
                .Where(r => r.CreateDay != null && r.CreateDay.Year >= startYear)
                .GroupBy(r => r.CreateDay.Year)
                .Select(g => new { Year = g.Key, Count = g.Count() })
                .ToListAsync();

            // Đảm bảo dữ liệu luôn có đủ 5 năm
            var studentsFinal = years.Select(y => studentData.FirstOrDefault(d => d.Year == y)?.Count ?? 0).ToList();
            var lecturersFinal = years.Select(y => lecturerData.FirstOrDefault(d => d.Year == y)?.Count ?? 0).ToList();
            var roomsFinal = years.Select(y => roomData.FirstOrDefault(d => d.Year == y)?.Count ?? 0).ToList();

            var data = new
            {
                years = years,
                students = studentsFinal,
                lecturers = lecturersFinal,
                rooms = roomsFinal
            };

            return Json(new { success = true, data });
        }
        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> report()
        {

            var lecturerNam = await _db.Lecturer
                 .AsNoTracking()
                 .Where(c => c.Gender && c.Status)
                 .GroupBy(l => l.FacultyId)
                 .Select(g => new { facultyId = g.Key, count = g.Count() })
                 .ToListAsync();
            var lecturerNu = await _db.Lecturer
                 .AsNoTracking()
                 .Where(c => !c.Gender && c.Status)
                 .GroupBy(l => l.FacultyId)
                 .Select(g => new { facultyId = g.Key, count = g.Count() })
                 .ToListAsync();
            var faculty = await _db.Faculty
                .AsNoTracking()
                .Where(f => f.Status)
                .Select(f => f.FacultyName)
                .ToListAsync();

            var facultyId = await _db.Faculty
                .AsNoTracking()
                .Where(f => f.Status)
                .Select(f => f.Id)
                .ToListAsync();

            var lecturerNamdata = facultyId.Select(f => lecturerNam.FirstOrDefault(l => l.facultyId == f)?.count ?? 0).ToList();
            var lecturerNudata = facultyId.Select(f => lecturerNu.FirstOrDefault(l => l.facultyId == f)?.count ?? 0).ToList();

            var data = new
            {
                Faculty = faculty,
                LecturerNam = lecturerNamdata,
                LecturerNu = lecturerNudata
            };

            return Json(new { success = true, data });
        }
        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> report2()
        {

            var studentNam = await _db.Student
                 .AsNoTracking()
                 .Include(c => c.Class)
                 .Where(c => c.Gender && c.Status)
                 .GroupBy(l => l.Class!.FacultyId)
                 .Select(g => new { facultyId = g.Key, count = g.Count() })
                 .ToListAsync();
            var studentNu = await _db.Student
                 .AsNoTracking()
                 .Include(c => c.Class)
                 .Where(c => !c.Gender && c.Status)
                 .GroupBy(l => l.Class!.FacultyId)
                 .Select(g => new { facultyId = g.Key, count = g.Count() })
                 .ToListAsync();
            var faculty = await _db.Faculty
                .AsNoTracking()
                .Where(f => f.Status)
                .Select(f => f.FacultyName)
                .ToListAsync();

            var facultyId = await _db.Faculty
                .AsNoTracking()
                .Where(f => f.Status)
                .Select(f => f.Id)
                .ToListAsync();

            var studentNamdata = facultyId.Select(f => studentNam.FirstOrDefault(l => l.facultyId == f)?.count ?? 0).ToList();
            var studentNudata = facultyId.Select(f => studentNu.FirstOrDefault(l => l.facultyId == f)?.count ?? 0).ToList();

            var data = new
            {
                Faculty = faculty,
                StudentNam = studentNamdata,
                StudentNu = studentNudata
            };

            return Json(new { success = true, data });
        }

        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> reportCountStu(int fillter = 0)
        {
            var currentYear = DateTime.UtcNow.Year;
            var priorYear = currentYear - 1;
            var StudentCurent = 0;
            var StudentPri = 0;
            if (fillter == 1)
            {
                 StudentCurent = await _db.Student
                    .AsNoTracking()
                    .CountAsync();
                 StudentPri = await _db.Student
                    .AsNoTracking()
                    .CountAsync();
            }
            else 
            {
                 StudentCurent = await _db.Student
                    .AsNoTracking()
                    .Where(c => c.CreateDay.Year == currentYear)
                    .CountAsync();
                 StudentPri = await _db.Student
                    .AsNoTracking()
                    .Where(c => c.CreateDay.Year == priorYear)
                    .CountAsync();
            }
            

            var percentIn = 0.0;

            if (StudentPri > 0)
            {
                percentIn = ((double)(StudentCurent - StudentPri) / StudentPri) * 100;
            }
            else if (StudentCurent > 0)
            {
                // Trường hợp năm trước chưa có sinh viên, năm nay có => tăng 100%
                percentIn = 100.0;
            }
            else
            {
                // Cả hai năm đều không có sinh viên
                percentIn = 0.0;
            }

            var data = new
            {
                StudentCurent,
                StudentPri,
                percentIn
            };
            return Json(new { success = true, data });
        }
        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> reportCountLeturer(int fillter = 0)
        {
            var currentYear = DateTime.UtcNow.Year;
            var priorYear = currentYear - 1;
            var LecturerCurent = 0;
            var LecturerPri = 0;
            if (fillter == 1)
            {
                LecturerCurent = await _db.Lecturer
                    .AsNoTracking()
                    .CountAsync();
                LecturerPri = await _db.Lecturer
                    .AsNoTracking()
                    .CountAsync();
            }
            else
            {
                LecturerCurent = await _db.Lecturer
                    .AsNoTracking()
                    .Where(c => c.CreateDay.Year == currentYear)
                    .CountAsync();
                LecturerPri = await _db.Lecturer
                    .AsNoTracking()
                    .Where(c => c.CreateDay.Year == priorYear)
                    .CountAsync();
            }

            var percentIn = 0.0;

            if (LecturerPri > 0)
            {
                percentIn = ((double)(LecturerCurent - LecturerPri) / LecturerPri) * 100;
            }
            else if (LecturerCurent > 0)
            {
                percentIn = 100.0;
            }
            else
            {
                percentIn = 0.0;
            }

            var data = new
            {
                LecturerCurent,
                LecturerPri,
                percentIn
            };

            return Json(new { success = true, data });
        }

        [Authorize(Policy = "NotLectureOrStudent")]
        public async Task<JsonResult> reportCountRoom(int fillter = 0)
        {
            var currentYear = DateTime.UtcNow.Year;
            var priorYear = currentYear - 1;
            var RoomCurent = 0;
            var RoomPri = 0;
            if (fillter == 1)
            {
                RoomCurent = await _db.Room
                    .AsNoTracking()
                    .CountAsync();
                RoomPri = await _db.Room
                    .AsNoTracking()
                    .CountAsync();
            }
            else
            {
                RoomCurent = await _db.Room
                    .AsNoTracking()
                    .Where(c => c.CreateDay.Year == currentYear)
                    .CountAsync();
                RoomPri = await _db.Room
                    .AsNoTracking()
                    .Where(c => c.CreateDay.Year == priorYear)
                    .CountAsync();
            }

            var percentIn = 0.0;

            if (RoomPri > 0)
            {
                percentIn = ((double)(RoomCurent - RoomPri) / RoomPri) * 100;
            }
            else if (RoomCurent > 0)
            {
                percentIn = 100.0;
            }
            else
            {
                percentIn = 0.0;
            }

            var data = new
            {
                RoomCurent,
                RoomPri,
                percentIn
            };

            return Json(new { success = true, data });
        }


        public async Task<IActionResult> AccountTB()
        {
            if (!User.Identity!.IsAuthenticated)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Account", new { area = "" }); // Điều hướng đến trang đăng nhập
            }

            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập!";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var user = await _db.Account
                .AsNoTracking()
                .Include(x => x.Lecturer)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(u => u.Username == username);
            return PartialView(user);
        }

        [Authorize]
        public async Task<ActionResult> EditPassword(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Account obj = await _db.Account.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id) ?? new Account();
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            return PartialView(obj);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> EditPasswordPost([Bind("Id,Password")] Models.Account obj, int? Id, string PasswordAgain = "", string PasswordCurent = "")
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

            if (!PasswordAgain.Equals(obj.Password))
            {
                return Json(new { success = false, message = "Mật khẩu nhập lại không giống" });
            }
            if (string.IsNullOrEmpty(PasswordCurent) || !new PasswordHelper().VerifyPassword(objData.Password, PasswordCurent))
            {
                return Json(new { success = false, message = "Mật khẩu không chính xác" });
            }

            // Kiểm tra độ mạnh của mật khẩu
            if (!IsStrongPassword(obj.Password))
            {
                return Json(new { success = false, message = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt" });
            }

            try
            {
                objData.Password = new PasswordHelper().HashPassword(obj.Password);

                await _db.SaveChangesAsync();

                _logger.LogInformation($"[{User.Identity?.Name}] Đã đổi mật khẩu cho tài khoản: Id = {objData.Id}");
                return Json(new { success = true, message = "Đổi mật khẩu thành công" });
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
                _logger.LogError(ex, $"[{User.Identity?.Name}] Lỗi khi đổi mật khẩu: {objData.Username}");
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }

        // Hàm kiểm tra độ mạnh mật khẩu
        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));
            bool hasMinLength = password.Length >= 8;

            return hasUpper && hasLower && hasDigit && hasSpecial && hasMinLength;
        }

    }
}
