using System;
using System.Linq;
using BTLWNCao.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTLWNCao.Controllers
{
    public class CongTyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CongTyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Xử lý POST khi tạo công ty
        [HttpPost]
        public IActionResult TaoCongTy(CongTyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra UserId trong Session
                    int maUser = HttpContext.Session.GetInt32("UserId") ?? 0;
                    if (maUser == 0)
                    {
                        // Nếu không có UserId trong session, chuyển hướng đến trang đăng nhập
                        return RedirectToAction("Login", "Account"); // Điều chỉnh tên Action/Controller theo thực tế
                    }

                    // Tạo đối tượng Công Ty từ ViewModel
                    var congTy = new CongTy
                    {
                        TenCongTy = model.TenCongTy,
                        SoDienThoai = model.SoDienThoai,
                        ThongTinCongTy = model.ThongTinCongTy,
                    };

                    // Lưu công ty vào cơ sở dữ liệu
                    _context.CongTys.Add(congTy);
                    _context.SaveChanges();

                    // Liên kết công ty với người dùng
                    _context.UserCongTys.Add(
                        new UserCongTy
                        {
                            MaCongTy = congTy.MaCongTy,
                            MaUser = maUser,
                            ChucVu = "Admin", // Gán quyền Admin cho người tạo công ty
                        }
                    );
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("CompanyId", congTy.MaCongTy);

                    // Sau khi tạo công ty, chuyển hướng đến trang phân quyền
                    return RedirectToAction("Index", "UserCongTy");
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            // Nếu form không hợp lệ, hiển thị lại thông báo lỗi
            return View(model);
        }

        // Xử lý GET khi hiển thị form tạo công ty
        [HttpGet]
        public IActionResult TaoCongTy()
        {
            return View(new CongTyViewModel());
        }
    }
}
