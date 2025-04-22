using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using WebUniversity.Models;



namespace WebUniversity.Controllers
{
    public class AccountController : Controller
    {
        private readonly DBContext _db;

        public AccountController(DBContext db)
        {
            _db = db;
        }

        public IActionResult login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAction(string UserName, string PassWord)
        {
            var user = _db.Account.Include(u => u.RoleGroup).FirstOrDefault(u => u.Username == UserName);

            if (user == null)
            {
                return Json(new { success = false, message = "Tài khoản không tồn tại" });
            }
            else if (!user.Status)
            {
                return Json(new { success = false, message = "Tài khoản đã bị khóa" });
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrEmpty(PassWord) || !new PasswordHelper().VerifyPassword(user.Password, PassWord))
            {
                return Json(new { success = false, message = "Mật khẩu không chính xác" });
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                };

            // Thêm role nếu có
            if (user.RoleGroup != null)
            {
                user.RoleGroup.RoleInRoleGroups = _db.RoleInRoleGroup.Where(r => r.RoleGroupId == user.RoleGroupId).Include(r => r.Role).ToList();
                foreach (var role in user.RoleGroup.RoleInRoleGroups)
                {
                    if (role.RoleGroup?.RoleInRoleGroups != null) { 
                        claims.Add(new Claim(ClaimTypes.Role, role.Role.RoleCode));
                    }
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            try
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true, // Giữ đăng nhập sau khi đóng trình duyệt
                        ExpiresUtc = DateTime.UtcNow.AddHours(12), // Thời gian hết hạn cookie
                        AllowRefresh = true
                    });

                string redirectUrl;
                if (user.LecturerId != null)
                {
                    redirectUrl = Url.Action("Index", "Info", new { area = "Lecturer" });
                }
                else if (user.StudentId != null)
                {
                    redirectUrl = Url.Action("Index", "Info", new { area = "Student" });
                }
                else
                {
                    redirectUrl = Url.Action("Index", "DashBoard", new { area = "Admin" });
                }

                return Json(new { success = true, url = redirectUrl });

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = $"Lỗi hệ thống: {e.Message}" });
            }
        }

        public async Task<IActionResult> Logout()
        {
            // Đăng xuất
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa session nếu có sử dụng
            HttpContext.Session.Clear();

            // Chuyển hướng về trang đăng nhập
            return RedirectToAction("Login", "Account");
        }

    }
}

