using Microsoft.AspNetCore.Mvc;
using BTLWNCao.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace BTLWNCao.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string TenDangNhap, string MatKhau)
        {
            var user = _context.Users.FirstOrDefault(u => u.TenDangNhap == TenDangNhap && u.MatKhau == MatKhau);

            if (user != null)
            {
                // Lưu thông tin User vào session
                HttpContext.Session.SetInt32("UserId", user.MaUser);
                HttpContext.Session.SetString("UserName", user.TenUser);

                // Truy vấn bảng UserCongTy để lấy MaUserCongTy
                var userCongTy = _context.UserCongTys.FirstOrDefault(uct => uct.MaUser == user.MaUser);
                if (userCongTy != null)
                {
                    HttpContext.Session.SetString("MaUserCongTy", userCongTy.MaUserCongTy.ToString());
                }

                // ✅ Chuyển tới trang tạo nhóm chat
                return RedirectToAction("ChiTiet", "NhomChat");
            }
            else
            {
                ViewBag.ThongBao = "Sai tên đăng nhập hoặc mật khẩu.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
