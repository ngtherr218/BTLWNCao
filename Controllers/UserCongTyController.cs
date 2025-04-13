using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BTLWNCao.Models;
using BTLWNCao.ViewModels; // <-- Đảm bảo using ViewModel đúng
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BTLWNCao.Controllers
{
    public class UserCongTyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserCongTyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Action Index (Không đổi) ---
        public IActionResult Index()
        {
            int? maCongTy = HttpContext.Session.GetInt32("CompanyId");
            if (maCongTy == null)
                return RedirectToAction("Login", "Login");

            ViewBag.CompanyId = maCongTy; // Vẫn có thể giữ lại nếu cần ở đâu đó khác
            var usersInCompany = _context.UserCongTys
                                .Include(uc => uc.User)
                                .Where(uc => uc.MaCongTy == maCongTy)
                                .ToList();
            return View(usersInCompany);
        }

        // --- Action Cập nhật (Đã bỏ ValidateAntiForgeryToken) ---
        [HttpPost]
        // [ValidateAntiForgeryToken] // <-- ĐÃ XÓA
        public IActionResult CapNhatChucVu(int maUserCongTy, string chucVu)
        {
            var userCongTy = _context.UserCongTys.Find(maUserCongTy);
            if (userCongTy != null)
            {
                int? currentCompanyId = HttpContext.Session.GetInt32("CompanyId");
                 if (userCongTy.MaCongTy != currentCompanyId)
                 {
                      return Json(new { success = false, message = "Hành động không được phép." });
                 }

                userCongTy.ChucVu = chucVu;
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Không tìm thấy người dùng trong công ty." });
        }

        // --- Action Xóa (Đã bỏ ValidateAntiForgeryToken) ---
        [HttpPost]
        // [ValidateAntiForgeryToken] // <-- ĐÃ XÓA
        public IActionResult DeleteEmployee(int maUserCongTy)
        {
            var userCongTy = _context.UserCongTys.Find(maUserCongTy);
            if (userCongTy != null)
            {
                int? currentCompanyId = HttpContext.Session.GetInt32("CompanyId");
                if (userCongTy.MaCongTy != currentCompanyId)
                {
                     return Json(new { success = false, message = "Hành động không được phép." });
                }

                _context.UserCongTys.Remove(userCongTy);
                try
                {
                    _context.SaveChanges();
                    return Json(new { success = true });
                }
                catch (DbUpdateException ex)
                {
                    Console.WriteLine($"DbUpdateException deleting UserCongTy {maUserCongTy}: {ex.ToString()}");
                    return Json(new { success = false, message = "Đã xảy ra lỗi khi xóa." });
                }
            }
            return Json(new { success = false, message = "Không tìm thấy người dùng để xóa." });
        }


        // --- Action AddEmployee GET (ĐÃ SỬA ĐỂ DÙNG VIEWMODEL) ---
        [HttpGet]
        public IActionResult AddEmployee()
        {
            int? maCongTy = HttpContext.Session.GetInt32("CompanyId");
            if (maCongTy == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Tạo ViewModel thay vì dùng ViewBag
            var viewModel = new AddEmployeeViewModel();

            // Lấy danh sách User và Roles, gán vào ViewModel
            var usersAlreadyInCompany = _context.UserCongTys
                                                .Where(uc => uc.MaCongTy == maCongTy)
                                                .Select(uc => uc.MaUser)
                                                .ToList();
            viewModel.AvailableUsers = _context.Users
                                        .Where(u => !usersAlreadyInCompany.Contains(u.MaUser))
                                        .Select(u => new SelectListItem
                                        {
                                            Value = u.MaUser.ToString(),
                                            Text = $"{u.TenUser} ({u.TenDangNhap})"
                                        })
                                        .ToList();

            viewModel.Roles = new List<SelectListItem>
            {
                // Thêm một lựa chọn trống để Required validation hoạt động tốt hơn
                // new SelectListItem { Value = "", Text = "-- Chọn chức vụ --" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                new SelectListItem { Value = "Quản lý", Text = "Quản lý" },
                new SelectListItem { Value = "Admin", Text = "Admin" }
            };

            // Trả về View với ViewModel đã được điền dữ liệu
            return View(viewModel);
        }

        // --- Action AddEmployee POST (ĐÃ SỬA ĐỂ NHẬN VÀ XỬ LÝ VIEWMODEL) ---
        [HttpPost]
        // [ValidateAntiForgeryToken] // <-- ĐÃ XÓA
        public IActionResult AddEmployee(AddEmployeeViewModel model) // <-- Nhận ViewModel thay vì int userId, string chucVu
        {
            int? maCongTy = HttpContext.Session.GetInt32("CompanyId");
            if (maCongTy == null)
            {
                ModelState.AddModelError("", "Phiên làm việc hết hạn hoặc không tìm thấy công ty.");
                // Cần chuẩn bị lại dropdowns cho ViewModel trước khi trả về View
                PrepareDropdownsForViewModel(model, maCongTy);
                return View(model); // Trả về View với ViewModel và lỗi
            }

            // Kiểm tra ModelState (dựa trên [Required] trong ViewModel) và các logic khác
            // Lưu ý: UserId trong ViewModel là int?, nên cần kiểm tra HasValue hoặc dùng Value
             if (model.UserId == null) // Kiểm tra null tường minh nếu cần (Required đã làm việc này)
             {
                  ModelState.AddModelError(nameof(model.UserId),"Vui lòng chọn người dùng.");
             }
             else // Chỉ kiểm tra logic phụ nếu UserId có giá trị
             {
                 var userExists = _context.Users.Any(u => u.MaUser == model.UserId.Value);
                 if (!userExists)
                 {
                     ModelState.AddModelError(nameof(model.UserId), "Người dùng được chọn không hợp lệ.");
                 }
                 var alreadyInCompany = _context.UserCongTys.Any(uc => uc.MaUser == model.UserId.Value && uc.MaCongTy == maCongTy);
                 if (alreadyInCompany)
                 {
                     ModelState.AddModelError("", "Người dùng này đã có trong công ty.");
                 }
             }
             // Kiểm tra ChucVu (Required đã làm việc này, kiểm tra giá trị nếu cần)
             if (string.IsNullOrEmpty(model.ChucVu))
             {
                  ModelState.AddModelError(nameof(model.ChucVu),"Vui lòng chọn chức vụ.");
             }
             else if (!new[] { "Admin", "Quản lý", "Nhân viên" }.Contains(model.ChucVu))
             {
                  ModelState.AddModelError(nameof(model.ChucVu), "Chức vụ không hợp lệ.");
             }


            // Nếu ModelState hợp lệ (bao gồm cả kiểm tra logic phụ)
            if (ModelState.IsValid)
            {
                var newUserCongTy = new UserCongTy
                {
                    MaUser = model.UserId.Value, // Lấy giá trị từ ViewModel
                    MaCongTy = maCongTy.Value,
                    ChucVu = model.ChucVu // Lấy giá trị từ ViewModel
                };

                _context.UserCongTys.Add(newUserCongTy);
                try
                {
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Thêm nhân viên thành công!"; // Thông báo thành công
                    return RedirectToAction("Index"); // Quay lại trang danh sách
                }
                catch (DbUpdateException ex)
                {
                     Console.WriteLine($"DbUpdateException adding UserCongTy: {ex.ToString()}"); // Log lỗi
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu vào cơ sở dữ liệu.");
                    // Chuẩn bị lại dropdowns trước khi trả về View lỗi
                    PrepareDropdownsForViewModel(model, maCongTy);
                    return View(model); // Trả về View với ViewModel và lỗi
                }
            }

            // Nếu ModelState không hợp lệ ngay từ đầu hoặc sau khi kiểm tra logic
            // Chuẩn bị lại dropdowns cho ViewModel trước khi trả về View
            PrepareDropdownsForViewModel(model, maCongTy);
            return View(model); // Trả về View với ViewModel và các lỗi ModelState
        }

        // --- Hàm trợ giúp MỚI để chuẩn bị Dropdowns cho ViewModel ---
        private void PrepareDropdownsForViewModel(AddEmployeeViewModel model, int? maCongTy)
        {
             if (maCongTy.HasValue)
             {
                 var usersAlreadyInCompany = _context.UserCongTys
                                                     .Where(uc => uc.MaCongTy == maCongTy)
                                                     .Select(uc => uc.MaUser)
                                                     .ToList();
                 // Gán trực tiếp vào thuộc tính của ViewModel được truyền vào
                 model.AvailableUsers = _context.Users
                                             .Where(u => !usersAlreadyInCompany.Contains(u.MaUser))
                                             .Select(u => new SelectListItem
                                             {
                                                 Value = u.MaUser.ToString(),
                                                 Text = $"{u.TenUser} ({u.TenDangNhap})"
                                             })
                                             .ToList();
             }
             else
             {
                 model.AvailableUsers = new List<SelectListItem>();
             }

             // Gán trực tiếp vào thuộc tính của ViewModel
             model.Roles = new List<SelectListItem>
             {
                 // new SelectListItem { Value = "", Text = "-- Chọn chức vụ --" },
                 new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                 new SelectListItem { Value = "Quản lý", Text = "Quản lý" },
                 new SelectListItem { Value = "Admin", Text = "Admin" }
             };
             // Không cần trả về gì vì đang thao tác trực tiếp trên đối tượng model được truyền vào
        }
    }
}