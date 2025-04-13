using BTLWNCao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTLWNCao
{
    public class DuAnController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DuAnController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int maCongTy, int? maDuAn)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var maUserCongTy = await _context
                .UserCongTys.Where(uct => uct.MaUser == userId && uct.MaCongTy == maCongTy)
                .Select(uct => uct.MaUserCongTy)
                .FirstOrDefaultAsync();

            if (maUserCongTy == 0)
            {
                return BadRequest(
                    "Không tìm thấy thông tin công ty hoặc người dùng trong hệ thống."
                );
            }

            var chucVu = await _context
                .UserCongTys.Where(uct => uct.MaUserCongTy == maUserCongTy)
                .Select(uct => uct.ChucVu)
                .FirstOrDefaultAsync();

            ViewBag.ChucVu = chucVu;
            ViewBag.MaDuAn = maDuAn;
            ViewBag.MaCongTy = maCongTy;

            var projects = await _context.DuAns.Where(p => p.MaCongTy == maCongTy).ToListAsync();

            return View(projects);
        }

        // GET: DuAn/CreateProject
        public IActionResult CreateProject(int maCongTy)
        {
            // Retrieve the User ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the User's Company role (MaUserCongTy) based on session userId and maCongTy
            var userCongTy = _context
                .UserCongTys.Where(uct => uct.MaUser == userId && uct.MaCongTy == maCongTy)
                .FirstOrDefault();

            if (userCongTy == null)
            {
                return BadRequest(
                    "Không tìm thấy thông tin công ty hoặc người dùng trong hệ thống."
                );
            }

            // Pass MaCongTy to the view so that it can be used in the form
            ViewBag.MaCongTy = maCongTy;
            return View();
        }

        // POST: DuAn/CreateProject
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProject(
            int maCongTy,
            string tenDuAn,
            string noiDungDuAn
        )
        {
            // Retrieve the User ID from the session
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            // Retrieve the User's Company role (MaUserCongTy) based on session userId and maCongTy
            var userCongTy = await _context
                .UserCongTys.Where(uct => uct.MaUser == userId && uct.MaCongTy == maCongTy)
                .FirstOrDefaultAsync();

            if (userCongTy == null)
            {
                return BadRequest(
                    "Không tìm thấy thông tin công ty hoặc người dùng trong hệ thống."
                );
            }

            // Create a new project instance
            var duAn = new DuAn
            {
                MaCongTy = maCongTy,
                MaUserCongTy = userCongTy.MaUserCongTy,
                TenDuAn = tenDuAn,
                NoiDungDuAn = noiDungDuAn,
            };

            // Add the new project to the context
            _context.DuAns.Add(duAn);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Redirect to the project list or project details page
            return RedirectToAction("Index", "DuAn", new { maCongTy = maCongTy });
        }
    }
}
