using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Data
{
    [Table("VongQuayVoucher")] // <-- MAP ĐÚNG TÊN BẢNG TRONG SQL
    public class VongQuayVoucher
    {
        [Key]
        public int Id { get; set; }

        public string TenO { get; set; }          // Tên hiển thị trên ô
        public int? MaVoucher { get; set; }       // Voucher trúng, có thể null
        public int TiLeTrung { get; set; }        // Trọng số (số càng lớn càng dễ trúng)
        public string MauHex { get; set; }        // Màu hiển thị cho đẹp
        public bool HoatDong { get; set; }        // Bật/tắt ô này

        // Nav properties
        [ForeignKey("MaVoucher")]
        public virtual Voucher Voucher { get; set; }

        public virtual ICollection<LuotQuayVoucher> LuotQuayVouchers { get; set; }
    }
}
