using System.ComponentModel.DataAnnotations;

namespace WebBanSach.Models.Data
{
    public enum TrangThaiDonHang
    {
        [Display(Name = "Chờ xác nhận")]
        ChoXacNhan = 0,

        [Display(Name = "Đã xác nhận")]
        DaXacNhan = 1,

        [Display(Name = "Đang giao hàng")]
        DangGiaoHang = 2,

        [Display(Name = "Đã giao")]
        DaGiao = 3,

        [Display(Name = "Đã hủy")]
        DaHuy = 4
    }
}
