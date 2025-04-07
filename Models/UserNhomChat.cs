using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BTLWNCao.Models
{
    public class UserNhomChat
    {
        [Key]
        public int MaUserNhomChat { get; set; }

        [Required]
        public int MaNhomChat { get; set; }

        [ForeignKey("MaNhomChat")]
        public NhomChat NhomChat { get; set; }

        [Required]
        public int MaUserCongTy { get; set; }

        [ForeignKey("MaUserCongTy")]
        public UserCongTy UserCongTy { get; set; }

        public ICollection<TinNhan> TinNhans { get; set; }
    }
}
