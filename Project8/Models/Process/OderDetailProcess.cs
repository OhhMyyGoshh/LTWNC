using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Process
{
    public class OderDetailProcess
    {
        BSDBContext db = null;
        public OderDetailProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// hàm lấy mã chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ChiTietDDH GetIdOrderDetail(int id)
        {
            return db.ChiTietDDHs.Find(id);
        }

        /// <summary>
        /// Xem chi tiết đơn hàng
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>List</returns>
        public List<ChiTietDDH> ListDetail(int id)
        {
            return db.ChiTietDDHs.Where(x => x.MaDDH == id).OrderBy(x => x.MaDDH).ToList();
        }

        /// <summary>
        /// hàm thêm sản phẩm vào đơn đặt hàng
        /// </summary>
        /// <param name="detail">ChiTietDDH</param>
        /// <returns>bool</returns>
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