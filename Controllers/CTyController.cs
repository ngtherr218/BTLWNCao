using BTLWNCao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BTLWNCao.Controllers
{
    public class CTyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CTyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            var companies = await _context
                .UserCongTys.Where(uc => uc.MaUser == userId.Value)
                .Include(uc => uc.CongTy)
                .Select(uc => uc.CongTy)
                .ToListAsync();

            return View(companies);
        }
    }
}
