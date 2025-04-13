using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTLWNCao.Models
{
    [Table("NhomChats")]
    public class NhomChat
    {
        [Key]
        public int MaNhomChat { get; set; }

        [Required]
        public int MaUserCongTy { get; set; }

        [ForeignKey("MaUserCongTy")]
        public UserCongTy UserCongTy { get; set; }

        [Required, StringLength(100)]
        public string TenNhomChat { get; set; }

        public ICollection<UserNhomChat> UserNhomChats { get; set; }
        public ICollection<TinNhan> TinNhans { get; set; }
    }
}
