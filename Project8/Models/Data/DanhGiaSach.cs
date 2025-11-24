using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Data
{
    [Table("DanhGiaSach")]
    public class DanhGiaSach
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }   // hoặc MaDanhGia nếu anh thích

        [Required]
        public int MaSach { get; set; }

        [Required]
        public int MaKH { get; set; }

        [Range(1, 5)]
        public int SoSao { get; set; }

        [StringLength(500)]
        public string NoiDung { get; set; }  // nội dung bình luận

        public DateTime NgayDanhGia { get; set; }

        public bool TrangThaiDuyet { get; set; } = false;

        // Quan hệ khóa ngoại
        [ForeignKey("MaSach")]
        public virtual Sach Sach { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }
    }
}
