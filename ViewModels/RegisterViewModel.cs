using System.ComponentModel.DataAnnotations;

namespace BTLWNCao.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(50)]
        public string TenDangNhap { get; set; }

        [Required, StringLength(255)]
        public string MatKhau { get; set; }

        [Required, StringLength(255)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu nhập lại không khớp.")]
        public string MatKhauNhapLai { get; set; }

        [Required, StringLength(15)]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Số điện thoại chỉ chứa các ký tự số.")]
        public string SoDienThoai { get; set; }

        [Required, StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
