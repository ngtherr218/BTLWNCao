using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTLWNCao.Models
{
    public class TinNhan
    {
        [Key]
        public int MaChat { get; set; }

        [Required]
        public int MaUserNhomChat { get; set; }

        [ForeignKey("MaUserNhomChat")]
        public UserNhomChat UserNhomChat { get; set; }

        [Required]
        public int MaNhomChat { get; set; }

        [ForeignKey("MaNhomChat")]
        public NhomChat NhomChat { get; set; }

        public string? NoiDung { get; set; }

        [StringLength(255)]
        public string? Anh { get; set; }

        [StringLength(255)]
        public string? FileTaiLieu { get; set; }

        public DateTime ThoiGianGui { get; set; } = DateTime.Now;
    }
}
