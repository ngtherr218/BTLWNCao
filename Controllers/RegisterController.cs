using Microsoft.AspNetCore.Mvc;
using BTLWNCao.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace BTLWNCao.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên đăng nhập đã tồn tại chưa
                if (_context.Users.Any(u => u.TenDangNhap == model.TenDangNhap))
                {
                    ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                // Kiểm tra mật khẩu có đủ điều kiện không
                var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                if (!passwordRegex.IsMatch(model.MatKhau))
                {
                    ModelState.AddModelError("MatKhau", "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ cái hoa, số và ký tự đặc biệt.");
                    return View(model);
                }

                // Mã hóa mật khẩu
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.MatKhau);

                // Tạo đối tượng User mới
                var user = new User
                {
                    TenUser = model.TenDangNhap,
                    TenDangNhap = model.TenDangNhap,
                    MatKhau = hashedPassword,
                    SoDienThoai = model.SoDienThoai,
                    Email = model.Email
                };

                // Lưu vào CSDL
                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Login", "Login");
            }

            return View(model);
        }
    }
}
