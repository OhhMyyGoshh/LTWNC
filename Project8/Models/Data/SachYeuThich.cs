using System;
using WebBanSach.Models.Data;

public class SachYeuThich
{
    public int Id { get; set; }
    public int MaSach { get; set; }
    public int MaKH { get; set; }
    public DateTime? NgayThem { get; set; }
    public virtual Sach Sach { get; set; }
    public virtual KhachHang KhachHang { get; set; }
}
