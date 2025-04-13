using System.Linq;
using BCrypt.Net;
using BTLWNCao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            // Kiểm tra nếu người dùng tồn tại trong cơ sở dữ liệu
            var user = _context.Users.FirstOrDefault(u => u.TenDangNhap == TenDangNhap);

            if (user != null && !string.IsNullOrEmpty(user.MatKhau))
            {
                // Mật khẩu đúng, lưu session
                HttpContext.Session.SetInt32("UserId", user.MaUser);
                HttpContext.Session.SetString("TenUser", user.TenUser ?? user.TenDangNhap);

                HttpContext.Session.SetString("UserName", user.TenUser);

                // Kiểm tra người dùng có công ty hay không
                var userCongTy = _context
                    .UserCongTys.Include(uc => uc.CongTy)
                    .FirstOrDefault(uc => uc.MaUser == user.MaUser);

                if (userCongTy != null)
                {
                    // Lưu thông tin công ty và phân quyền
                    HttpContext.Session.SetInt32("CompanyId", userCongTy.MaCongTy);
                    HttpContext.Session.SetString("MaUserCongTy", userCongTy.MaUserCongTy.ToString());
                    HttpContext.Session.SetString("UserRole", userCongTy.ChucVu);
                    if (userCongTy.ChucVu == "Admin")
                        return RedirectToAction("Index", "UserCongTy"); // Trang phân quyền
                    else
                        return RedirectToAction("ChiTiet", "NhomChat"); // Trang chat cho nhân viên
                }
                else
                {
                    // Người dùng chưa có công ty, chuyển đến trang tạo công ty
                    return RedirectToAction("TaoCongTy", "CongTy");
                }
            }

            // Nếu tài khoản không tồn tại hoặc mật khẩu sai
            ViewBag.ThongBao = "Sai tên đăng nhập hoặc mật khẩu.";
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Xóa session khi người dùng đăng xuất
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
