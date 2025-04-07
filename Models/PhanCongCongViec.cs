using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTLWNCao.Models
{
    public class PhanCongCongViec
    {
        [Key]
        public int MaCongViec { get; set; }

        [Required]
        public int MaUserCongTy { get; set; }

        [ForeignKey("MaUserCongTy")]
        public UserCongTy UserCongTy { get; set; }

        [Required, StringLength(255)]
        public string TenCongViec { get; set; }

        public string NoiDung { get; set; }

        [StringLength(255)]
        public string Anh { get; set; }

        [StringLength(255)]
        public string FileTaiLieu { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        [Required]
        public int MaDuAn { get; set; }

        [ForeignKey("MaDuAn")]
        public DuAn DuAn { get; set; }

        [Required]
        public int MaCongTy { get; set; }

        [ForeignKey("MaCongTy")]
        public CongTy CongTy { get; set; } 
    }
}
