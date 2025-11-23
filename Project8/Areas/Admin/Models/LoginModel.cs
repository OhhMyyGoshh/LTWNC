using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Data;
using System.ComponentModel.DataAnnotations;

namespace WebBanSach.Areas.Admin.Models
{
    public class LoginModel
    {
        //Call a LoginModel to equal with Admin in Models

        [Display(Name ="Tài khoản")]
        public string TaiKhoan { get; set; }

        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; }

        [Display(Name = "Ghi nhớ")]
        public bool GhiNho { get; set; }

        [Display(Name = "Họ tên")]
        public string HoTen { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? TrangThai { get; set; }
    }
}