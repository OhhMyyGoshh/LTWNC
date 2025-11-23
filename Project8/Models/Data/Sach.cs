namespace WebBanSach.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Sach")]
    public partial class Sach
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Sach()
        {
            ChiTietDDHs = new HashSet<ChiTietDDH>();
        }

        [Key]
        [Display(Name = "Mã sách")]
        public int MaSach { get; set; }

        [Display(Name = "Mã loại")]
        [Required(ErrorMessage ="Vui lòng chọn thể loại")]
        public int MaLoai { get; set; }

        [Display(Name = "Mã NXB")]
        [Required(ErrorMessage ="Vui lòng chọn nhà xuất bản")]
        public int MaNXB { get; set; }

        [Display(Name = "Mã tác giả")]
        [Required(ErrorMessage = "Vui lòng chọn tác giả")]
        public int MaTG { get; set; }

        [StringLength(250)]
        [Display(Name = "Tên sách")]
        [Required(ErrorMessage = "Vui lòng nhập tên sách")]
        public string TenSach { get; set; }

        [Column(TypeName = "money")]
        [Display(Name = "Giá bán")]
        public decimal? GiaBan { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string Mota { get; set; }

        [StringLength(50)]
        [Display(Name = "Người dịch")]
        public string NguoiDich { get; set; }

        [StringLength(50)]
        [Display(Name = "Ảnh bìa")]
        public string AnhBia { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name = "Ngày cập nhật")]
        public DateTime? NgayCapNhat { get; set; }

        [Display(Name = "Số lượng tồn")]
        public int? SoLuongTon { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChiTietDDH> ChiTietDDHs { get; set; }

        public virtual NhaXuatBan NhaXuatBan { get; set; }

        public virtual TheLoai TheLoai { get; set; }

        public virtual TacGia TacGia { get; set; }
    }
}
