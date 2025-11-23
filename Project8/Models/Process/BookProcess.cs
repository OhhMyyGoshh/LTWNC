
using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace WebBanSach.Models.Process
{
    public class BookProcess
    {
        //Khởi tạo biến dữ liệu : db
        BSDBContext db = null;

        //constructor :  khởi tạo đối tượng
        public BookProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// Lấy danh sách sách bán chạy thực sự (dựa vào tổng số lượng bán ra, chỉ tính đơn đã hoàn thành)
        /// </summary>
        /// <param name="count">Số lượng sách muốn lấy</param>
        /// <returns>List<Sach></returns>
        public List<Sach> GetBestSellerBooks(int count)
        {
            // Lấy tất cả chi tiết đơn hàng thuộc các đơn đã giao (TrangThaiDonHang = 3)
            var bestSellers = db.ChiTietDDHs
                .Where(ct => ct.DonDatHang.TrangThaiDonHang == 3)
                .GroupBy(ct => ct.MaSach)
                .Select(g => new
                {
                    MaSach = g.Key,
                    SoLuongBan = g.Sum(x => x.SoLuong ?? 0)
                })
                .OrderByDescending(x => x.SoLuongBan)
                .Take(count)
                .ToList();

            if (!bestSellers.Any())
                return new List<Sach>();

            // Lấy thông tin sách tương ứng
            var sachIds = bestSellers.Select(x => x.MaSach).ToList();
            var books = db.Saches.Where(s => sachIds.Contains(s.MaSach)).ToList();
            // Đảm bảo thứ tự đúng theo số lượng bán
            return bestSellers.Select(x => books.FirstOrDefault(s => s.MaSach == x.MaSach)).Where(s => s != null).ToList();
        }

        /// <summary>
        /// lấy cuốn mới nhất theo ngày cập nhật
        /// </summary>
        /// <param name="count">int</param>
        /// <returns>List</returns>
        public List<Sach> NewDateBook(int count)
        {
            return db.Saches.OrderByDescending(x => x.NgayCapNhat).Take(count).ToList();
        }

        /// <summary>
        /// lọc sách theo chủ đề
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>List</returns>
        public List<Sach> ThemeBook(int id)
        {
            return db.Saches.Where(x => x.MaLoai == id).ToList();
        }

        /// <summary>
        /// Lấy sách chọn lọc
        /// </summary>
        /// <param name="count">int</param>
        /// <returns>List</returns>
        public List<Sach> TakeBook(int count)
        {
            return db.Saches.OrderBy(x => x.NgayCapNhat).Take(count).ToList();
        }

        /// <summary>
        /// Xem tất cả cuốn sách
        /// </summary>
        /// <returns>List</returns>
        public List<Sach> ShowAllBook()
        {
            return db.Saches.OrderBy(x => x.MaSach).ToList();
        }
    }
}