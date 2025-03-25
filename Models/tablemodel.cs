using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebUniversity.Models
{
    

    public class TopSaleViewModel
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; }
        public string HinhAnh { get; set; }
        public decimal DonGia { get; set; }
        public int SoLuongBan { get; set; }
        public decimal Revenue { get; set; }
    }

    public partial class Student
    {
        DBContext db = new DBContext();
        public Class? Classes => db.Class.Find(ClassId) ?? new Class();
    }

    public partial class Class
    {
        DBContext DBContext = new DBContext();
        public Faculty? FacultyS => DBContext.Faculty.Find(FacultyId) ?? new Faculty();
    }
    public partial class Lecturer
    {
        DBContext DBContext = new DBContext();
        public Faculty? FacultyS => DBContext.Faculty.Find(FacultyId) ?? new Faculty();
    }

    public partial class Subject
    {
        DBContext DBContext = new DBContext();
        public Faculty? FacultyS => DBContext.Faculty.Find(FacultyId) ?? new Faculty();
    }

    public partial class Course
    {
        DBContext DBContext = new DBContext();
        public Subject? SubjectS => DBContext.Subject.Find(SubjectId) ?? new Subject();
        public Lecturer? LecturerS => DBContext.Lecturer.Find(LecturerId) ?? new Lecturer();
    }

    public partial class Role
    {
        DBContext DBContext = new DBContext();
        public Role? RoleS => (ParentId != null) ? DBContext.Role.Find(ParentId) ?? new Role(): null;
    }



    public enum TrangThaiDonHang
    {
        [Display(Name = "Đang xử lý")]
        DangXuLy = 1,
        [Display(Name = "Đã hoàn thành")]
        DaHoanThanh = 2,
        [Display(Name = "Đang giao")]
        DaGiao = 3,
        [Display(Name = "Đã hủy")]
        DaHuy = 4
    }

}
