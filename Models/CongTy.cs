using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BTLWNCao.Models
{
    public class CongTy
    {
        [Key]
        public int MaCongTy { get; set; }

        [Required, StringLength(255)]
        public string TenCongTy { get; set; }

        [StringLength(15)]
        public string SoDienThoai { get; set; }

        public string ThongTinCongTy { get; set; }

        public ICollection<UserCongTy> UserCongTys { get; set; }
        public ICollection<DuAn> DuAns { get; set; }
    }
}
