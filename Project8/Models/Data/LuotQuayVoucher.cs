using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanSach.Models.Data
{
    [Table("LuotQuayVoucher")]
    public class LuotQuayVoucher
    {
        [Key]
        public int Id { get; set; }

        // Khách hàng nào quay
        public int MaKH { get; set; }

        // Ô nào trên vòng quay (foreign key tới VongQuayVoucher.Id)
        public int VongQuayVoucherId { get; set; }

        // Thời gian quay
        public DateTime NgayQuay { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; }

        [ForeignKey("VongQuayVoucherId")]
        public virtual VongQuayVoucher VongQuayVoucher { get; set; }
    }
}
