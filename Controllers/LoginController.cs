using System.Linq;
using BTLWNCao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var user = _context.Users.FirstOrDefault(u =>
                u.TenDangNhap == TenDangNhap && u.MatKhau == MatKhau
            );

            if (user != null)
            {
                // Lưu thông tin User vào session
                HttpContext.Session.SetInt32("UserId", user.MaUser);
                HttpContext.Session.SetString("UserName", user.TenUser);

                return RedirectToAction("Index", "Home");
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
