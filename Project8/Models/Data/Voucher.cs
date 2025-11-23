using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Data
{
    [Table("Voucher")]
    public partial class Voucher
    {
        [Key]
        public int MaVoucher { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã code")]
        [StringLength(20)]
        public string MaCode { get; set; }

        [StringLength(100)]
        [Display(Name = "Tên voucher")]
        public string TenVoucher { get; set; }

        [Display(Name = "Loại giảm giá")]
        public bool? LoaiGiamGia { get; set; } // 0: %, 1: Tiền

        [Display(Name = "Giá trị giảm")]
        public decimal? GiaTriGiam { get; set; }

        [Display(Name = "Giá trị đơn hàng tối thiểu")]
        public decimal? GiaTriDonHangToiThieu { get; set; }

        [Display(Name = "Số lượng")]
        public int? SoLuong { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime? NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? TrangThai { get; set; }
    }

    [Table("VoucherSuDung")]
    public partial class VoucherSuDung
    {
        [Key]
        public int ID { get; set; }

        public int MaVoucher { get; set; }
        public int MaKH { get; set; }
        public int MaDDH { get; set; }
        public DateTime? NgaySuDung { get; set; }

        [ForeignKey("MaVoucher")]
        public virtual Voucher Voucher { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("MaDDH")]
        public virtual DonDatHang DonDatHang { get; set; }
    }
}
