using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTLWNCao.Models
{
    public class CongTyViewModel
    {
    [Required(ErrorMessage = "Tên công ty không được để trống")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên công ty phải từ 2 đến 100 ký tự")]
    public string TenCongTy { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số, chỉ bao gồm số.")]
    public string SoDienThoai { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập thông tin công ty.")]
    public string ThongTinCongTy { get; set; }
}
}