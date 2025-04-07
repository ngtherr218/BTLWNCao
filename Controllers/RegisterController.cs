using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTLWNCao.Models;
using System.Linq;
using System.Threading.Tasks;
using BCrypt.Net;

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
    public IActionResult Register(string TenDangNhap, string MatKhau, string MatKhauNhapLai, string SoDienThoai, string Email)
    {
        // Kiểm tra mật khẩu nhập lại
        if (MatKhau != MatKhauNhapLai)
        {
            ModelState.AddModelError("MatKhauNhapLai", "Mật khẩu nhập lại không khớp.");
            return View();
        }

        // Kiểm tra tên đăng nhập đã tồn tại chưa
        if (_context.Users.Any(u => u.TenDangNhap == TenDangNhap))
        {
            ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại.");
            return View();
        }

        // Tạo đối tượng User mới
        var user = new User
        {
            TenUser = TenDangNhap, // Hoặc có thể tạo thêm input để nhập tên đầy đủ
            TenDangNhap = TenDangNhap,
            MatKhau = MatKhau, // Nên hash mật khẩu trong ứng dụng thực tế
            SoDienThoai = SoDienThoai,
            Email = Email
        };

        // Lưu vào CSDL
        _context.Users.Add(user);
        _context.SaveChanges();

        // Chuyển hướng sau khi đăng ký thành công
        return RedirectToAction("Login", "Login");
    }
}
