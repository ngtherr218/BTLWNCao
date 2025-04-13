using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTLWNCao.Models
{
    public class DuAn
    {
        [Key]
        public int MaDuAn { get; set; }

        [Required]
        public int MaUserCongTy { get; set; }

        [ForeignKey("MaUserCongTy")]
        public UserCongTy UserCongTy { get; set; }

        [Required]
        public int MaCongTy { get; set; }

        [ForeignKey("MaCongTy")]
        public CongTy CongTy { get; set; }

        [Required, StringLength(255)]
        public string TenDuAn { get; set; }

        public string? NoiDungDuAn { get; set; }

        public ICollection<PhanCongCongViec> PhanCongCongViecs { get; set; }
    }
}
