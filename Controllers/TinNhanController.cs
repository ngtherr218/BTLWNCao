using BTLWNCao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;

namespace BTLWNCao.Controllers
{
    public class TinNhanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TinNhanController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> GuiTinNhan(int maNhomChat, string? noiDung, IFormFile? file)
        {
            var maUserCongTyStr = HttpContext.Session.GetString("MaUserCongTy");
            if (string.IsNullOrEmpty(maUserCongTyStr) || !int.TryParse(maUserCongTyStr, out int maUserCongTy))
                return Unauthorized();

            var userNhom = await _context.UserNhomChats.FirstOrDefaultAsync(u =>
                u.MaUserCongTy == maUserCongTy && u.MaNhomChat == maNhomChat);

            if (userNhom == null)
                return Json(new { success = false, message = "Không tìm thấy user trong nhóm chat." });

            string? filePath = null;
            string? fileName = null;

            if (file != null && file.Length > 0)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                filePath = Path.Combine(uploadsFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }

            var tinNhan = new TinNhan
            {
                MaUserNhomChat = userNhom.MaUserNhomChat,
                MaNhomChat = maNhomChat,
                NoiDung = noiDung,
                ThoiGianGui = DateTime.Now,
                Anh = (file?.ContentType.StartsWith("image") == true) ? fileName : null,
                FileTaiLieu = (file?.ContentType.StartsWith("image") != true) ? fileName : null
            };

            try
            {
                _context.TinNhans.Add(tinNhan);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return Json(new { success = false, message = "Lỗi khi lưu tin nhắn: " + message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> LayTinNhanTheoNhom(int maNhomChat)
        {
            try
            {
                var tinNhans = await _context.TinNhans
                    .Where(t => t.MaNhomChat == maNhomChat)
                    .Include(t => t.UserNhomChat)
                    .ThenInclude(u => u.UserCongTy)
                    .ThenInclude(uc => uc.User)
                    .OrderBy(t => t.ThoiGianGui)
                    .Select(t => new
                    {
                        t.MaChat,
                        t.NoiDung,
                        t.ThoiGianGui,
                        TenNguoiGui = t.UserNhomChat.UserCongTy.User.TenUser,
                        t.Anh,
                        t.FileTaiLieu
                    })
                    .ToListAsync();

                if (tinNhans == null || tinNhans.Count == 0)
                {
                    return Json(new { success = false, message = "Không có tin nhắn nào." });
                }

                return Json(tinNhans);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Không thể lấy tin nhắn: " + ex.Message });
            }
        }
    }
}