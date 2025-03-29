
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUniversity.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;


namespace WebUniversity.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : Controller
    {
        private readonly DBContext _db;
        private const string KeyCache = "Course";

        public CourseController(DBContext db)
        {
            _db = db;
        }

        [Authorize(Roles = "Course|Course.View")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Course|Course.View")]
        [HttpGet]
        public async Task<PartialViewResult> ListData()
        {
            List<Models.Course> listData = null;
            try
            {
                var list = _db.Course.AsQueryable();

                listData = await list.OrderByDescending(g => g.CreateDay).ToListAsync();

            }
            catch (Exception ex)
            {
                
            }
            return PartialView(listData);
        }

        [Authorize(Roles = "Course|Course.View")]
        public async Task<ActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Không được để trống Id" });
            }
            Models.Course objData = await _db.Course.FindAsync(id);
            if (objData == null)
            {
                return Json(new { success = false, message = " Bản ghi không tồn tại" });
            }
            var listfaculty = _db.Course.ToList();
            ViewData["listfaculty"] = listfaculty;
            return PartialView(objData);
        }

        [Authorize(Roles = "Course|Course.Create")]
        public PartialViewResult Create()
        {
            var listsubject = _db.Subject.ToList();
            ViewData["listsubject"] = listsubject;
            var listlecturer = _db.Lecturer.ToList();
            ViewData["listlecturer"] = listlecturer;

            return PartialView();
        }

        [Authorize(Roles = "Course|Course.Create")]
        [HttpPost]
        public async Task<JsonResult> Create([Bind("Id,SubjectId,LecturerId,Semester,SchoolYear,Status")] Models.Course obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy dữ liệu môn học và giảng viên
                    var subject = _db.Subject.FirstOrDefault(s => s.Id == obj.SubjectId);
                    var lecturer = _db.Lecturer.FirstOrDefault(l => l.Id == obj.LecturerId);

                    // Tạo mã môn học (lấy chữ cái đầu của từng từ)
                    string subjectCode = subject != null
                        ? new string(subject.SubjectName.Split(' ').Select(w => w[0]).ToArray()).ToUpper()
                        : "UNK";

                    // Tạo mã giảng viên (lấy chữ cái đầu của từng từ)
                    string lecturerCode = lecturer != null
                        ? new string(lecturer.FullName.Split(' ').Select(w => w[0]).ToArray()).ToUpper()
                        : "UNK";

                    // Tạo CourseName theo định dạng mong muốn
                    obj.CourseName = $"{subjectCode}_{lecturerCode}_{obj.Semester}_{obj.SchoolYear}";

                    // Thêm vào database
                    _db.Course.Add(obj);
                    await _db.SaveChangesAsync();

                    return Json(new { success = true, message = "Thêm mới thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi dữ liệu nhập" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Thêm mới thất bại, vui lòng thử lại!" });
            }
        }


        [Authorize(Roles = "Course|Course.Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            Models.Course obj = await _db.Course.FindAsync(id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Bản ghi không tồn tại" });
            }

            var listsubject = _db.Subject.ToList();
            ViewData["listsubject"] = listsubject;
            var listlecturer = _db.Lecturer.ToList();
            ViewData["listlecturer"] = listlecturer;
            return PartialView(obj);
        }

        [Authorize(Roles = "Course|Course.Edit")]
        [HttpPost]
        public async Task<JsonResult> EditPost([Bind("Id,SubjectId,LecturerId,Semester,SchoolYear,Status")] Models.Course obj, int? Id)
        {
            if (Id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }

            var objData = await _db.Course.FindAsync(Id);
            if (objData == null)
            {
                return Json(new { success = false, message = "Không thể lưu vì có người dùng khác đang sửa hoặc đã bị xóa" });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Lấy dữ liệu môn học & giảng viên
                    var subject = _db.Subject.FirstOrDefault(s => s.Id == obj.SubjectId);
                    var lecturer = _db.Lecturer.FirstOrDefault(l => l.Id == obj.LecturerId);

                    // Tạo CourseName mới nếu SubjectId hoặc LecturerId thay đổi
                    string subjectCode = subject != null
                        ? new string(subject.SubjectName.Split(' ').Select(w => w[0]).ToArray()).ToUpper()
                        : "UNK";

                    string lecturerCode = lecturer != null
                        ? new string(lecturer.FullName.Split(' ').Select(w => w[0]).ToArray()).ToUpper()
                        : "UNK";

                    objData.CourseName = $"{subjectCode}_{lecturerCode}_{obj.Semester}_{obj.SchoolYear}";

                    // Cập nhật các thông tin khác
                    objData.SubjectId = obj.SubjectId;
                    objData.LecturerId = obj.LecturerId;
                    objData.Semester = obj.Semester;
                    objData.SchoolYear = obj.SchoolYear;
                    objData.Status = obj.Status;

                    await _db.SaveChangesAsync();
                    return Json(new { success = true, message = "Cập nhật thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }
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
            catch (Exception)
            {
                return Json(new { success = false, message = "Không thể lưu được" });
            }
        }


        [Authorize(Roles = "Course|Course.Delete")]
        public JsonResult Delete(int? id)
        {
            Course obj = null;

            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            try
            {
                obj = _db.Course.Find(id);
                if (obj != null)
                {
                    _db.Course.Remove(obj);
                    _db.SaveChanges();
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Json(new { success = false, message = "Không xóa được bản ghi này" });
            }

            return Json(new { success = true, message = "Bản ghi đã được xóa thành công" });
        }

        [Authorize(Roles = "Course|Course.Edit")]
        public async Task<JsonResult> Status(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id không được để trống" });
            }
            var objData = await _db.Course.FindAsync(id);
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