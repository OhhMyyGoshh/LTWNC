using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;

namespace WebBanSach.Models
{
    [Serializable]
    public class CartModel
    {
        public Sach sach { get; set; }
        public int Quantity { get; set; }
        public decimal? Total
        {
            get { return Quantity * sach.GiaBan; }
        }
    }
}