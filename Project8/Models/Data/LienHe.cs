namespace WebBanSach.Models.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LienHe")]
    public partial class LienHe
    {
        [Key]
        public int MaLH { get; set; }

        [StringLength(50)]
        [Display(Name ="Họ")]
        [Required(ErrorMessage ="Họ không được để trống")]
        public string Ho { get; set; }

        [StringLength(50)]
        [Display(Name = "Tên")]
        [Required(ErrorMessage ="Tên không được để trống")]
        public string Ten { get; set; }

        [StringLength(100)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email không được để trống")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(50)]
        [Display(Name = "Điện thoại")]
        [Required(ErrorMessage = "Không được để trống số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public string DienThoai { get; set; }

        [StringLength(500)]
        [Display(Name = "Nội dung")]
        [Required(ErrorMessage = "Nội dung cần nhập của bạn")]
        public string NoiDung { get; set; }

        [Column(TypeName = "smalldatetime")]
        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.DateTime)]
        public DateTime? NgayCapNhat { get; set; }
    }
}
