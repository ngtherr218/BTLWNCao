using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTLWNCao.Models
{
    public class UserCongTy
    {
        [Key]
        public int MaUserCongTy { get; set; }

        [Required]
        public int MaCongTy { get; set; }

        [ForeignKey("MaCongTy")]
        public CongTy CongTy { get; set; }

        [Required]
        public int MaUser { get; set; }

        [ForeignKey("MaUser")]
        public User User { get; set; }

        [StringLength(100)]
        public string ChucVu { get; set; }

        public ICollection<NhomChat> NhomChats { get; set; }
        public ICollection<UserNhomChat> UserNhomChats { get; set; }
        public ICollection<DuAn> DuAns { get; set; }
        public ICollection<PhanCongCongViec> PhanCongCongViecs { get; set; }
    }
}
