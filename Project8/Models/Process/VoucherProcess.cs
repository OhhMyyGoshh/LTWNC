using System;
using System.Collections.Generic;
using System.Linq;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Process
{
    public class VoucherProcess
    {
        BSDBContext db = new BSDBContext();

        // Kiểm tra và áp dụng voucher
        public VoucherResult ApplyVoucher(string maCode, decimal tongTien, int? maKH = null)
        {
            var result = new VoucherResult();

            try
            {
                // Tìm voucher theo mã code
                var voucher = db.Vouchers.FirstOrDefault(v => 
                    v.MaCode.ToLower() == maCode.ToLower() && 
                    v.TrangThai == true);

                if (voucher == null)
                {
                    result.Success = false;
                    result.Message = "Mã voucher không tồn tại hoặc đã hết hạn!";
                    return result;
                }

                // Kiểm tra thời gian
                var now = DateTime.Now;
                if (voucher.NgayBatDau > now || voucher.NgayKetThuc < now)
                {
                    result.Success = false;
                    result.Message = "Mã voucher chưa có hiệu lực hoặc đã hết hạn!";
                    return result;
                }

                // Kiểm tra số lượng
                if (voucher.SoLuong <= 0)
                {
                    result.Success = false;
                    result.Message = "Mã voucher đã hết số lượng!";
                    return result;
                }

                // Kiểm tra giá trị đơn hàng tối thiểu
                if (tongTien < voucher.GiaTriDonHangToiThieu)
                {
                    result.Success = false;
                    result.Message = $"Đơn hàng phải từ {voucher.GiaTriDonHangToiThieu:N0}đ trở lên!";
                    return result;
                }

                // Tính giá trị giảm
                decimal giamGia = 0;
                if (voucher.LoaiGiamGia == false) // Giảm theo %
                {
                    giamGia = tongTien * (voucher.GiaTriGiam ?? 0) / 100;
                }
                else // Giảm cố định
                {
                    giamGia = voucher.GiaTriGiam ?? 0;
                }

                // Không cho giảm quá tổng tiền
                if (giamGia > tongTien)
                {
                    giamGia = tongTien;
                }

                result.Success = true;
                result.Message = "Áp dụng mã giảm giá thành công!";
                result.MaVoucher = voucher.MaVoucher;
                result.GiamGia = giamGia;
                result.TongTienSauGiam = tongTien - giamGia;

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Có lỗi xảy ra: " + ex.Message;
                return result;
            }
        }

        // Giảm số lượng voucher khi sử dụng
        public bool UseVoucher(int maVoucher, int maKH, int maDDH)
        {
            try
            {
                var voucher = db.Vouchers.Find(maVoucher);
                if (voucher != null && voucher.SoLuong > 0)
                {
                    // Giảm số lượng
                    voucher.SoLuong--;

                    // Lưu lịch sử sử dụng
                    var history = new VoucherSuDung
                    {
                        MaVoucher = maVoucher,
                        MaKH = maKH,
                        MaDDH = maDDH,
                        NgaySuDung = DateTime.Now
                    };
                    db.VoucherSuDungs.Add(history);

                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // Lấy danh sách voucher đang hoạt động
        public List<Voucher> GetActiveVouchers()
        {
            var now = DateTime.Now;
            return db.Vouchers
                .Where(v => v.TrangThai == true && 
                           v.NgayBatDau <= now && 
                           v.NgayKetThuc >= now && 
                           v.SoLuong > 0)
                .OrderBy(v => v.GiaTriDonHangToiThieu)
                .ToList();
        }

        // Admin: Lấy tất cả voucher
        public List<Voucher> GetAllVouchers()
        {
            return db.Vouchers.OrderByDescending(v => v.MaVoucher).ToList();
        }

        // Admin: Thêm voucher
        public int InsertVoucher(Voucher voucher)
        {
            try
            {
                db.Vouchers.Add(voucher);
                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        // Admin: Cập nhật voucher
        public int UpdateVoucher(Voucher voucher)
        {
            try
            {
                var v = db.Vouchers.Find(voucher.MaVoucher);
                if (v != null)
                {
                    v.MaCode = voucher.MaCode;
                    v.TenVoucher = voucher.TenVoucher;
                    v.LoaiGiamGia = voucher.LoaiGiamGia;
                    v.GiaTriGiam = voucher.GiaTriGiam;
                    v.GiaTriDonHangToiThieu = voucher.GiaTriDonHangToiThieu;
                    v.SoLuong = voucher.SoLuong;
                    v.NgayBatDau = voucher.NgayBatDau;
                    v.NgayKetThuc = voucher.NgayKetThuc;
                    v.TrangThai = voucher.TrangThai;

                    return db.SaveChanges();
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        // Admin: Xóa voucher
        public int DeleteVoucher(int id)
        {
            try
            {
                var voucher = db.Vouchers.Find(id);
                if (voucher != null)
                {
                    db.Vouchers.Remove(voucher);
                    return db.SaveChanges();
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }
    }

    // Class kết quả áp dụng voucher
    public class VoucherResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int MaVoucher { get; set; }
        public decimal GiamGia { get; set; }
        public decimal TongTienSauGiam { get; set; }
    }
}
