using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Models.Process
{
    public class HomeProcess
    {
        //Khởi tạo biến dữ liệu : db
        BSDBContext db = null;

        //constructor :  khởi tạo đối tượng
        public HomeProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// hàm xuất danh sách thể loại
        /// </summary>
        /// <returns></returns>
        public List<TheLoai> ListCategory()
        {
            return db.TheLoais.OrderBy(x => x.MaLoai).ToList();
        }

        /// <summary>
        /// hàm lưu phản hồi từ khách hàng vào db
        /// </summary>
        /// <param name="entity">LienHe</param>
        /// <returns>int</returns>
        public int InsertContact(LienHe entity)
        {
            db.LienHes.Add(entity);
            db.SaveChanges();

            return entity.MaLH;
        }

        /// <summary>
        /// hàm tìm kiếm tên sách
        /// </summary>
        /// <param name="key">string</param>
        /// <returns>List</returns>
        public List<Sach> Search(string key)
        {
            return db.Saches.Where(x => x.TenSach.Contains(key)).OrderBy(x=>x.TenSach).ToList();
        }

    }
}