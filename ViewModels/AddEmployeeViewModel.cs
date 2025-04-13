using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Cho SelectListItem

namespace BTLWNCao.ViewModels // Đảm bảo đúng namespace
{
    public class AddEmployeeViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn người dùng.")]
        [Display(Name = "Người Dùng")] // Tên hiển thị trên Label
        public int? UserId { get; set; } // Dùng int? để Required hoạt động tốt với <option value="">

        [Required(ErrorMessage = "Vui lòng chọn chức vụ.")]
        [Display(Name = "Chức Vụ")]
        public string ChucVu { get; set; }

        // Thêm các danh sách để chứa dữ liệu cho Dropdown
        public List<SelectListItem> AvailableUsers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}