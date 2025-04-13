using BTLWNCao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BTLWNCao.Controllers
{
    public class NhomChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NhomChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách nhóm chat của người dùng hiện tại
        public async Task<IActionResult> Index()
        {
            var maUserCongTyStr = HttpContext.Session.GetString("MaUserCongTy");

            if (string.IsNullOrEmpty(maUserCongTyStr) || !int.TryParse(maUserCongTyStr, out int maUserCongTy))
                return RedirectToAction("Login", "Login");

            var nhomChats = await _context.UserNhomChats
                .Include(unc => unc.NhomChat)
                .Where(unc => unc.MaUserCongTy == maUserCongTy)
                .Select(unc => unc.NhomChat)
                .Distinct()
                .ToListAsync();

            return View(nhomChats);
        }

        // Vào chi tiết 1 nhóm chat hoặc chưa chọn nhóm (id null)
        public async Task<IActionResult> ChiTiet(int? id)
        {
            var maUserCongTyStr = HttpContext.Session.GetString("MaUserCongTy");
            if (string.IsNullOrEmpty(maUserCongTyStr) || !int.TryParse(maUserCongTyStr, out int maUserCongTy))
                return RedirectToAction("Login", "Login");

            // Load sidebar: nhóm của tôi
            ViewBag.NhomCuaToi = await _context.UserNhomChats
                .Include(unc => unc.NhomChat)
                .Where(unc => unc.MaUserCongTy == maUserCongTy)
                .Select(unc => unc.NhomChat)
                .Distinct()
                .ToListAsync();

            // Nếu chưa chọn nhóm, trả về view với model null
            if (id == null)
            {
                return View(model: null);
            }

            // Kiểm tra nếu người dùng không thuộc nhóm => về trang chính
            var userNhom = await _context.UserNhomChats
                .FirstOrDefaultAsync(u => u.MaUserCongTy == maUserCongTy && u.MaNhomChat == id.Value);

            if (userNhom == null)
                return RedirectToAction("Index");

            var nhom = await _context.NhomChats
                .Include(n => n.UserNhomChats).ThenInclude(unc => unc.UserCongTy).ThenInclude(uc => uc.User)
                .Include(n => n.TinNhans).ThenInclude(t => t.UserNhomChat).ThenInclude(unc => unc.UserCongTy).ThenInclude(uc => uc.User)
                .FirstOrDefaultAsync(n => n.MaNhomChat == id.Value);

            if (nhom == null)
                return NotFound();

            // Gợi ý thành viên (chỉ khi đã chọn nhóm)
            var congTyId = _context.UserCongTys
                .Where(u => u.MaUserCongTy == maUserCongTy)
                .Select(u => u.MaCongTy)
                .FirstOrDefault();

            var userDaThamGia = nhom.UserNhomChats.Select(u => u.MaUserCongTy).ToList();

            var thanhVienGoiY = await _context.UserCongTys
                .Include(uc => uc.User)
                .Where(u => u.MaCongTy == congTyId && !userDaThamGia.Contains(u.MaUserCongTy))
                .ToListAsync();

            ViewBag.ThanhVienGoiY = thanhVienGoiY;

            return View(nhom);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // AJAX - tạo nhóm chat (chỉ Giám đốc/Trưởng phòng được tạo)
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Dictionary<string, string> data)
        {
            try
            {
                if (!data.TryGetValue("TenNhomChat", out string tenNhomChat) || string.IsNullOrWhiteSpace(tenNhomChat))
                {
                    return Json(new { success = false, message = "Tên nhóm không hợp lệ" });
                }

                var maUserCongTyStr = HttpContext.Session.GetString("MaUserCongTy");

                if (string.IsNullOrEmpty(maUserCongTyStr) || !int.TryParse(maUserCongTyStr, out int maUserCongTy))
                {
                    return Json(new { success = false, message = "Chưa đăng nhập hoặc không hợp lệ" });
                }

                // Kiểm tra chức vụ người dùng
                var chucVu = await _context.UserCongTys
                    .Where(u => u.MaUserCongTy == maUserCongTy)
                    .Select(u => u.ChucVu)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(chucVu) || !(chucVu == "Giám đốc" || chucVu == "Trưởng phòng" || chucVu == "Quản lý"))
                {
                    return Json(new { success = false, message = "Bạn không có quyền tạo nhóm" });
                }

                var nhom = new NhomChat
                {
                    TenNhomChat = tenNhomChat,
                    MaUserCongTy = maUserCongTy
                };

                _context.NhomChats.Add(nhom);
                await _context.SaveChangesAsync();

                var userNhom = new UserNhomChat
                {
                    MaUserCongTy = maUserCongTy,
                    MaNhomChat = nhom.MaNhomChat
                };

                _context.UserNhomChats.Add(userNhom);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Tạo nhóm thành công!",
                    data = new
                    {
                        maNhomChat = nhom.MaNhomChat,
                        tenNhomChat = nhom.TenNhomChat
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Lỗi khi tạo nhóm: " + ex.Message
                });
            }
        }

        // Thêm thành viên vào nhóm (chỉ Giám đốc/Trưởng phòng/Quản lý được phép)
        [HttpPost]
        public async Task<IActionResult> ThemThanhVien(int maNhomChat, int maUserCongTy)
        {
            // Kiểm tra chức vụ của người dùng
            var maUserCongTyStr = HttpContext.Session.GetString("MaUserCongTy");
            if (string.IsNullOrEmpty(maUserCongTyStr) || !int.TryParse(maUserCongTyStr, out int currentUserCongTy))
                return Json(new { success = false, message = "Chưa đăng nhập hoặc không hợp lệ" });

            var chucVu = await _context.UserCongTys
                .Where(u => u.MaUserCongTy == currentUserCongTy)
                .Select(u => u.ChucVu)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(chucVu) || !(chucVu == "Giám đốc" || chucVu == "Trưởng phòng" || chucVu == "Quản lý"))
            {
                return Json(new { success = false, message = "Bạn không có quyền thêm thành viên vào nhóm" });
            }

            // Kiểm tra xem người dùng đã là thành viên trong nhóm chưa
            var daCo = await _context.UserNhomChats
                .AnyAsync(u => u.MaNhomChat == maNhomChat && u.MaUserCongTy == maUserCongTy);
            if (daCo)
                return Json(new { success = false, message = "Người dùng đã trong nhóm" });

            var userNhom = new UserNhomChat
            {
                MaNhomChat = maNhomChat,
                MaUserCongTy = maUserCongTy
            };

            _context.UserNhomChats.Add(userNhom);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã thêm thành viên" });
        }

        // Xóa thành viên khỏi nhóm (chỉ Giám đốc/Trưởng phòng/Quản lý có quyền)
        [HttpPost]
        public async Task<IActionResult> XoaThanhVien(int maNhomChat, int maUserCongTy)
        {
            // Kiểm tra chức vụ của người dùng
            var maUserCongTyStr = HttpContext.Session.GetString("MaUserCongTy");
            if (string.IsNullOrEmpty(maUserCongTyStr) || !int.TryParse(maUserCongTyStr, out int currentUserCongTy))
                return Json(new { success = false, message = "Chưa đăng nhập hoặc không hợp lệ" });

            var chucVu = await _context.UserCongTys
                .Where(u => u.MaUserCongTy == currentUserCongTy)
                .Select(u => u.ChucVu)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(chucVu) || !(chucVu == "Giám đốc" || chucVu == "Trưởng phòng" || chucVu == "Quản lý"))
            {
                return Json(new { success = false, message = "Bạn không có quyền xóa thành viên khỏi nhóm" });
            }

            var thanhVien = await _context.UserNhomChats
                .FirstOrDefaultAsync(u => u.MaNhomChat == maNhomChat && u.MaUserCongTy == maUserCongTy);

            if (thanhVien == null)
                return Json(new { success = false, message = "Thành viên không tồn tại trong nhóm" });

            _context.UserNhomChats.Remove(thanhVien);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã xóa thành viên khỏi nhóm" });
        }
    }
}
