using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebBanSach.Models.Process
{
    public class OrderProcess
    {
        //khởi tạo dữ liệu từ tầng data
        BSDBContext db = null;

        //contructor
        public OrderProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// hàm lấy mã đơn đặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DonDatHang GetIdOrder(int id)
        {
            return db.DonDatHangs.Find(id);
        }

        /// <summary>
        /// hàm xuất danh sách đơn đặt hàng
        /// </summary>
        /// <returns>List</returns>
        public List<DonDatHang> ListOrder()
        {
            return db.DonDatHangs.OrderBy(x => x.MaDDH).ToList();
        }

        /// <summary>
        /// hàm thêm đơn hàng
        /// </summary>
        /// <param name="order">DonDatHang</param>
        /// <returns>int</returns>
        public int Insert(DonDatHang order)
        {
            db.DonDatHangs.Add(order);
            db.SaveChanges();
            return order.MaDDH;
        }
    }
}