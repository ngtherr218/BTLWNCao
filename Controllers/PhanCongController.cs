using BTLWNCao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTLWNCao.Controllers
{
    public class PhanCongController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhanCongController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int maDuAn, int maCongTy)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return RedirectToAction("Login", "Login");

            var maUserCongTy = await _context
                .UserCongTys.Where(uct => uct.MaUser == userId && uct.MaCongTy == maCongTy)
                .Select(uct => uct.MaUserCongTy)
                .FirstOrDefaultAsync();

            if (maUserCongTy == 0)
                return BadRequest(
                    "Không tìm thấy thông tin công ty hoặc người dùng trong hệ thống."
                );

            var chucVu = await _context
                .UserCongTys.Where(uct => uct.MaUserCongTy == maUserCongTy)
                .Select(uct => uct.ChucVu)
                .FirstOrDefaultAsync();

            ViewBag.ChucVu = chucVu;
            ViewBag.MaDuAn = maDuAn;
            ViewBag.MaCongTy = maCongTy;

            List<PhanCongCongViec> danhSach;

            if (chucVu == "Quản lý")
            {
                danhSach = await _context
                    .PhanCongCongViecs.Include(pc => pc.UserCongTy)
                    .ThenInclude(uct => uct.User)
                    .Where(pc => pc.MaDuAn == maDuAn)
                    .ToListAsync();
            }
            else
            {
                danhSach = await _context
                    .PhanCongCongViecs.Include(pc => pc.UserCongTy)
                    .ThenInclude(uct => uct.User)
                    .Where(pc => pc.MaDuAn == maDuAn && pc.MaUserCongTy == maUserCongTy)
                    .ToListAsync();
            }

            return View(danhSach);
        }

        public IActionResult Create()
        {
            ViewData["UserCongTy"] = _context.UserCongTys.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            PhanCongCongViec model,
            IFormFile Anh,
            IFormFile FileTaiLieu
        )
        {
            if (ModelState.IsValid)
            {
                if (Anh != null && Anh.Length > 0)
                {
                    var fileName = Path.GetFileName(Anh.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await Anh.CopyToAsync(stream);
                    }
                    model.Anh = "/images/" + fileName;
                }

                if (FileTaiLieu != null && FileTaiLieu.Length > 0)
                {
                    var fileName = Path.GetFileName(FileTaiLieu.FileName);
                    var path = Path.Combine("wwwroot/files", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await FileTaiLieu.CopyToAsync(stream);
                    }
                    model.FileTaiLieu = "/files/" + fileName;
                }

                _context.PhanCongCongViecs.Add(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, errors = errors });
        }

        [HttpGet]
        public JsonResult GetUsersBySessionCompany()
        {
            int? maCongTy = int.Parse(Request.Query["maCongTy"]);
            if (maCongTy == null)
                return Json(new { error = "Chưa chọn công ty" });

            var users = _context
                .UserCongTys.Where(u => u.MaCongTy == maCongTy)
                .Select(u => new
                {
                    u.MaUserCongTy,
                    Ten = _context.Users.FirstOrDefault(x => x.MaUser == u.MaUser).TenUser,
                })
                .ToList();

            return Json(users);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id)
        {
            var congViec = await _context
                .PhanCongCongViecs.Include(p => p.UserCongTy)
                .ThenInclude(uct => uct.User)
                .FirstOrDefaultAsync(c => c.MaCongViec == id);

            if (congViec == null)
                return NotFound();

            return View(congViec);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(
            PhanCongCongViec model,
            IFormFile Anh,
            IFormFile FileTaiLieu
        )
        {
            if (ModelState.IsValid)
            {
                if (Anh != null && Anh.Length > 0)
                {
                    var fileName = Path.GetFileName(Anh.FileName);
                    var path = Path.Combine("wwwroot/images", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await Anh.CopyToAsync(stream);
                    }
                    model.Anh = "/images/" + fileName;
                }

                if (FileTaiLieu != null && FileTaiLieu.Length > 0)
                {
                    var fileName = Path.GetFileName(FileTaiLieu.FileName);
                    var path = Path.Combine("wwwroot/files", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await FileTaiLieu.CopyToAsync(stream);
                    }
                    model.FileTaiLieu = "/files/" + fileName;
                }

                _context.PhanCongCongViecs.Update(model);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }

            var errors = ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return Json(new { success = false, errors = errors });
        }

        public async Task<IActionResult> DeleteConfirmed(int id, int maDuAn, int maCongTy)
        {
            var congViec = await _context.PhanCongCongViecs.FindAsync(id);
            if (congViec != null)
            {
                _context.PhanCongCongViecs.Remove(congViec);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { maDuAn = maDuAn, maCongTy = maCongTy });
        }
    }
}
