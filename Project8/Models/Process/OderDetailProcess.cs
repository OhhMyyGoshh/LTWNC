using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;   // THÊM DÒNG NÀY

namespace WebBanSach.Models.Process
{
    public class OderDetailProcess
    {
        BSDBContext db = null;
        public OderDetailProcess()
        {
            db = new BSDBContext();
        }

        public ChiTietDDH GetIdOrderDetail(int id)
        {
            return db.ChiTietDDHs.Find(id);
        }

        /// <summary>
        /// Xem chi tiết đơn hàng (kèm thông tin Sách + Đơn + Khách)
        /// </summary>
        public List<ChiTietDDH> ListDetail(int id)
        {
            return db.ChiTietDDHs
                     .Include(x => x.Sach)
                     .Include(x => x.DonDatHang)
                     .Include(x => x.DonDatHang.KhachHang)
                     .Where(x => x.MaDDH == id)
                     .OrderBy(x => x.MaDDH)
                     .ToList();
        }

        public bool Insert(ChiTietDDH detail)
        {
            try
            {
                db.ChiTietDDHs.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
