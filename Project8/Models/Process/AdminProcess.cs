using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Process
{
    public class AdminProcess
    {
        //Tầng xử lý dữ liệu

        BSDBContext db = null;

        //constructor
        public AdminProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// Hàm đăng nhập
        /// </summary>
        /// <param name="username">string</param>
        /// <param name="password">string</param>
        /// <returns>int</returns>
        public int Login(string username, string password)
        {
            var result = db.Admins.SingleOrDefault(x => x.TaiKhoan == username);
            if (result == null)
            {
                return 0;
            }
            else
            {
                if (result.MatKhau == password)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        //Get ID : lấy mã

        #region lấy mã

        /// <summary>
        /// hàm lấy mã admin
        /// </summary>
        public Admin GetIdAdmin(int id)
        {
            return db.Admins.Find(id);
        }

        /// <summary>
        /// hàm lấy mã sách
        /// </summary>
        public Sach GetIdBook(int id)
        {
            return db.Saches.Find(id);
        }

        /// <summary>
        /// hàm lấy mã thể loại
        /// </summary>
        public TheLoai GetIdCategory(int id)
        {
            return db.TheLoais.Find(id);
        }

        /// <summary>
        /// hàm lấy mã tác giả
        /// </summary>
        public TacGia GetIdAuthor(int id)
        {
            return db.TacGias.Find(id);
        }

        /// <summary>
        /// hàm lấy mã nhà xuất bản
        /// </summary>
        public NhaXuatBan GetIdPublish(int id)
        {
            return db.NhaXuatBans.Find(id);
        }

        /// <summary>
        /// Hàm lấy mã khách hàng tham quan
        /// </summary>
        public KhachHang GetIdCustomer(int id)
        {
            return db.KhachHangs.Find(id);
        }

        /// <summary>
        /// hàm lấy mã đơn đặt hàng
        /// </summary>
        public DonDatHang GetIdOrder(int id)
        {
            return db.DonDatHangs.Find(id);
        }

        /// <summary>
        /// hàm lấy mã liên hệ
        /// </summary>
        public LienHe GetIdContact(int id)
        {
            return db.LienHes.Find(id);
        }

        #endregion

        //Category : thể loại

        #region thể loại

        /// <summary>
        /// hàm xuất danh sách thể loại
        /// </summary>
        public List<TheLoai> ListAllCategory()
        {
            return db.TheLoais.OrderBy(x => x.MaLoai).ToList();
        }

        /// <summary>
        /// hàm thêm thểm loại
        /// </summary>
        public int InsertCategory(TheLoai entity)
        {
            db.TheLoais.Add(entity);
            db.SaveChanges();
            return entity.MaLoai;
        }

        /// <summary>
        /// hàm cập nhật thể loại
        /// </summary>
        public int UpdateCategory(TheLoai entity)
        {
            try
            {
                var tl = db.TheLoais.Find(entity.MaLoai);
                tl.TenLoai = entity.TenLoai;
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// hàm xóa thể loại
        /// </summary>
        public bool DeleteCategory(int id)
        {
            try
            {
                var tl = db.TheLoais.Find(id);
                db.TheLoais.Remove(tl);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        //Author : tác giả

        #region tác giả

        /// <summary>
        /// hàm xuất danh sách tác giả
        /// </summary>
        public List<TacGia> ListAllAuthor()
        {
            return db.TacGias.OrderBy(x => x.MaTG).ToList();
        }

        /// <summary>
        /// hàm thêm tác giả
        /// </summary>
        public int InsertAuthor(TacGia entity)
        {
            db.TacGias.Add(entity);
            db.SaveChanges();
            return entity.MaTG;
        }

        /// <summary>
        /// hàm cập nhật tác giả
        /// </summary>
        public int UpdateAuthor(TacGia entity)
        {
            try
            {
                var tg = db.TacGias.Find(entity.MaTG);
                tg.TenTG = entity.TenTG;
                tg.QueQuan = entity.QueQuan;
                tg.NgaySinh = entity.NgaySinh;
                tg.NgayMat = entity.NgayMat;
                tg.TieuSu = entity.TieuSu;
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// hàm xóa tác giả
        /// </summary>
        public bool DeleteAuthor(int id)
        {
            try
            {
                var tg = db.TacGias.Find(id);
                db.TacGias.Remove(tg);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        //Publish : nhà xuất bản

        #region nhà xuất bản

        /// <summary>
        /// hàm xuất danh sách nhà xuất bản
        /// </summary>
        public List<NhaXuatBan> ListAllPublish()
        {
            return db.NhaXuatBans.OrderBy(x => x.MaNXB).ToList();
        }

        /// <summary>
        /// hàm thêm nhà xuất bản
        /// </summary>
        public int InsertPublish(NhaXuatBan entity)
        {
            db.NhaXuatBans.Add(entity);
            db.SaveChanges();
            return entity.MaNXB;
        }

        /// <summary>
        /// hàm cập nhật nhà xuất bản
        /// </summary>
        public int UpdatePublish(NhaXuatBan entity)
        {
            try
            {
                var nxb = db.NhaXuatBans.Find(entity.MaNXB);
                nxb.TenNXB = entity.TenNXB;
                nxb.DiaChi = entity.DiaChi;
                nxb.DienThoai = entity.DienThoai;
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// hàm xóa nhà xuất bản
        /// </summary>
        public bool DeletePublish(int id)
        {
            try
            {
                var nxb = db.NhaXuatBans.Find(id);
                db.NhaXuatBans.Remove(nxb);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        //Books : sách

        #region sách

        /// <summary>
        /// hàm xuất danh sách Sách
        /// </summary>
        public List<Sach> ListAllBook()
        {
            return db.Saches.OrderBy(x => x.MaSach).ToList();
        }

        /// <summary>
        /// hàm thêm sách
        /// </summary>
        public int InsertBook(Sach entity)
        {
            db.Saches.Add(entity);
            db.SaveChanges();
            return entity.MaSach;
        }

        /// <summary>
        /// hàm cập nhật sách
        /// </summary>
        public int UpdateBook(Sach entity)
        {
            try
            {
                var sach = db.Saches.Find(entity.MaSach);
                sach.MaLoai = entity.MaLoai;
                sach.MaNXB = entity.MaNXB;
                sach.MaTG = entity.MaTG;
                sach.TenSach = entity.TenSach;
                sach.GiaBan = entity.GiaBan;
                sach.Mota = entity.Mota;
                sach.NguoiDich = entity.NguoiDich;
                sach.AnhBia = entity.AnhBia;
                sach.NgayCapNhat = entity.NgayCapNhat;
                sach.SoLuongTon = entity.SoLuongTon;
                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// hàm xóa 1 cuốn sách
        /// </summary>
        public bool DeleteBook(int id)
        {
            try
            {
                var sach = db.Saches.SingleOrDefault(x => x.MaSach == id);
                db.Saches.Remove(sach);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        //Liên hệ từ khách hàng

        #region phản hồi khách hàng

        /// <summary>
        /// hàm lấy danh sách những phản hồi từ khách hàng
        /// </summary>
        public List<LienHe> ShowListContact()
        {
            return db.LienHes.OrderBy(x => x.MaLH).ToList();
        }

        /// <summary>
        /// hàm xóa thông tin phản hồi khách hàng
        /// </summary>
        public bool deleteContact(int id)
        {
            try
            {
                var contact = db.LienHes.Find(id);
                db.LienHes.Remove(contact);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        //Quản lý người dùng

        /// <summary>
        /// hàm xuất danh sách người dùng
        /// </summary>
        public List<KhachHang> ListUser()
        {
            return db.KhachHangs.OrderBy(x => x.MaKH).ToList();
        }

        /// <summary>
        /// hàm thay đổi trạng thái người dùng (kích hoạt / khóa)
        /// </summary>
        public bool ChangeStatusUser(int id)
        {
            var kh = db.KhachHangs.SingleOrDefault(x => x.MaKH == id);
            if (kh == null) return false;

            kh.TrangThai = !kh.TrangThai;

            db.SaveChanges();

            return kh.TrangThai;
        }

        /// <summary>
        /// hàm xóa người dùng
        /// </summary>
        public bool DeleteUser(int id)
        {
            try
            {
                var user = db.KhachHangs.Find(id);
                db.KhachHangs.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //ĐƠN ĐẶT HÀNG

        #region đơn đặt hàng

        /// <summary>
        /// Cập nhật trạng thái đơn hàng, nếu chuyển sang ĐÃ GIAO (3) thì trừ tồn kho
        /// </summary>
        public bool UpdateOrderStatusAndStock(int orderId, int newStatus, out string message)
        {
            message = "";

            var order = db.DonDatHangs.SingleOrDefault(x => x.MaDDH == orderId);
            if (order == null)
            {
                message = "Không tìm thấy đơn hàng.";
                return false;
            }

            int oldStatus = order.TrangThaiDonHang ?? 0;

            // Chỉ trừ tồn khi chuyển từ trạng thái KHÁC sang ĐÃ GIAO (3)
            if (newStatus == 3 && oldStatus != 3)
            {
                var chiTiet = db.ChiTietDDHs
                                .Where(x => x.MaDDH == orderId)
                                .ToList();

                // 1. CHECK tồn kho trước
                foreach (var ct in chiTiet)
                {
                    var sach = db.Saches.SingleOrDefault(s => s.MaSach == ct.MaSach);
                    if (sach == null) continue;

                    int soTon = sach.SoLuongTon ?? 0;
                    int soLuongDat = ct.SoLuong ?? 0;

                    if (soLuongDat > soTon)
                    {
                        message = "Sách '" + sach.TenSach + "' chỉ còn " + soTon + " bản, không đủ để giao " + soLuongDat + " bản.";
                        return false;
                    }
                }

                // 2. ĐỦ tồn → trừ tồn
                foreach (var ct in chiTiet)
                {
                    var sach = db.Saches.SingleOrDefault(s => s.MaSach == ct.MaSach);
                    if (sach == null) continue;

                    int soTon = sach.SoLuongTon ?? 0;
                    int soLuongDat = ct.SoLuong ?? 0;

                    sach.SoLuongTon = soTon - soLuongDat;
                }
            }

            // Cập nhật trạng thái đơn
            order.TrangThaiDonHang = newStatus;
            db.SaveChanges();

            if (newStatus == 3 && oldStatus != 3)
                message = "Cập nhật trạng thái ĐÃ GIAO và trừ tồn kho thành công.";
            else
                message = "Cập nhật trạng thái đơn hàng thành công.";

            return true;
        }

        #endregion

        //VOUCHER : mã giảm giá

        #region voucher

        /// <summary>
        /// Lấy 1 voucher theo ID
        /// </summary>
        public Voucher GetIdVoucher(int id)
        {
            return db.Vouchers.Find(id);
        }

        /// <summary>
        /// Danh sách tất cả voucher
        /// </summary>
        public List<Voucher> ListAllVoucher()
        {
            return db.Vouchers
                     .OrderByDescending(x => x.NgayKetThuc)
                     .ToList();
        }

        /// <summary>
        /// Thêm voucher mới
        /// </summary>
        public int InsertVoucher(Voucher entity)
        {
            db.Vouchers.Add(entity);
            db.SaveChanges();
            return entity.MaVoucher;
        }

        /// <summary>
        /// Cập nhật voucher
        /// </summary>
        public int UpdateVoucher(Voucher entity)
        {
            try
            {
                var vc = db.Vouchers.Find(entity.MaVoucher);
                if (vc == null) return 0;

                vc.MaCode = entity.MaCode;
                vc.TenVoucher = entity.TenVoucher;
                vc.LoaiGiamGia = entity.LoaiGiamGia;
                vc.GiaTriGiam = entity.GiaTriGiam;
                vc.GiaTriDonHangToiThieu = entity.GiaTriDonHangToiThieu;
                vc.SoLuong = entity.SoLuong;
                vc.NgayBatDau = entity.NgayBatDau;
                vc.NgayKetThuc = entity.NgayKetThuc;
                vc.TrangThai = entity.TrangThai;

                db.SaveChanges();
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Xóa voucher
        /// </summary>
        public bool DeleteVoucher(int id)
        {
            try
            {
                var vc = db.Vouchers.Find(id);
                if (vc == null) return false;

                db.Vouchers.Remove(vc);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}
