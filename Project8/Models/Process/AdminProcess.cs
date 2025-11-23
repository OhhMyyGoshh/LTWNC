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
        /// <param name="id">int</param>
        /// <returns>Admin</returns>
        public Admin GetIdAdmin(int id)
        {
            return db.Admins.Find(id);
        }

        /// <summary>
        /// hàm lấy mã sách
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>Sach</returns>
        public Sach GetIdBook(int id)
        {
            return db.Saches.Find(id);
        }

        /// <summary>
        /// hàm lấy mã thể loại
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>TheLoai</returns>
        public TheLoai GetIdCategory(int id)
        {
            return db.TheLoais.Find(id);
        }

        /// <summary>
        /// hàm lấy mã tác giả
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>TacGia</returns>
        public TacGia GetIdAuthor(int id)
        {
            return db.TacGias.Find(id);
        }

        /// <summary>
        /// hàm lấy mã nhà xuất bản
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>NhaXuatBan</returns>
        public NhaXuatBan GetIdPublish(int id)
        {
            return db.NhaXuatBans.Find(id);
        }

        /// <summary>
        /// Hàm lấy mã khách hàng tham quan
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>KhachHang</returns>
        public KhachHang GetIdCustomer(int id)
        {
            return db.KhachHangs.Find(id);
        }

        /// <summary>
        /// hàm lấy mã đơn đặt hàng
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>DonDatHang</returns>
        public DonDatHang GetIdOrder(int id)
        {
            return db.DonDatHangs.Find(id);
        }

        /// <summary>
        /// hàm lấy mã liên hệ
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>LienHe</returns>
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
        /// <returns>List</returns>
        public List<TheLoai> ListAllCategory()
        {
            return db.TheLoais.OrderBy(x => x.MaLoai).ToList();
        }

        /// <summary>
        /// hàm thêm thểm loại
        /// </summary>
        /// <param name="entity">TheLoai</param>
        /// <returns>int</returns>
        public int InsertCategory(TheLoai entity)
        {
            db.TheLoais.Add(entity);
            db.SaveChanges();
            return entity.MaLoai;
        }

        /// <summary>
        /// hàm cập nhật thể loại
        /// </summary>
        /// <param name="entity">TheLoai</param>
        /// <returns>int</returns>
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
        /// <param name="id">int</param>
        /// <returns>bool</returns>
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
        /// <returns>List</returns>
        public List<TacGia> ListAllAuthor()
        {
            return db.TacGias.OrderBy(x => x.MaTG).ToList();
        }

        /// <summary>
        /// hàm thêm tác giả
        /// </summary>
        /// <param name="entity">TacGia</param>
        /// <returns></returns>
        public int InsertAuthor(TacGia entity)
        {
            db.TacGias.Add(entity);
            db.SaveChanges();
            return entity.MaTG;
        }

        /// <summary>
        /// hàm cập nhật tác giả
        /// </summary>
        /// <param name="entity">TacGia</param>
        /// <returns>int</returns>
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
        /// <param name="id">int</param>
        /// <returns>int</returns>
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
        /// <returns>List</returns>
        public List<NhaXuatBan> ListAllPublish()
        {
            return db.NhaXuatBans.OrderBy(x => x.MaNXB).ToList();
        }

        /// <summary>
        /// hàm thêm nhà xuất bản
        /// </summary>
        /// <param name="entity">NhaXuatBan</param>
        /// <returns>int</returns>
        public int InsertPublish(NhaXuatBan entity)
        {
            db.NhaXuatBans.Add(entity);
            db.SaveChanges();
            return entity.MaNXB;
        }

        /// <summary>
        /// hàm cập nhật nhà xuất bản
        /// </summary>
        /// <param name="entity">NhaXuatBan</param>
        /// <returns>int</returns>
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
        /// <param name="id">int</param>
        /// <returns>bool</returns>
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
        /// <returns>List</returns>
        public List<Sach> ListAllBook()
        {
            return db.Saches.OrderBy(x => x.MaSach).ToList();
        }

        /// <summary>
        /// hàm thêm sách
        /// </summary>
        /// <param name="entity">Sach</param>
        /// <returns>int</returns>
        public int InsertBook(Sach entity)
        {
            db.Saches.Add(entity);
            db.SaveChanges();
            return entity.MaSach;
        }

        /// <summary>
        /// hàm cập nhật sách
        /// </summary>
        /// <param name="entity">Sách</param>
        /// <returns>int</returns>
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
        /// <param name="id">int</param>
        /// <returns>bool</returns>
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
        /// <returns>List</returns>
        public List<LienHe> ShowListContact()
        {
            return db.LienHes.OrderBy(x => x.MaLH).ToList();
        }

        /// <summary>
        /// hàm xóa thông tin phản hồi khách hàng
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>bool</returns>
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
        /// <returns>List</returns>
        public List<KhachHang> ListUser()
        {
            return db.KhachHangs.OrderBy(x => x.MaKH).ToList();
        }

        /// <summary>
        /// hàm xóa người dùng
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>bool</returns>
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

    }
}