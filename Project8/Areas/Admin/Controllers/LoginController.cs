using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Process;
using WebBanSach.Areas.Admin.Models;
using WebBanSach.Models.Data;

namespace WebBanSach.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        //Trang đăng nhập admin

        //Khởi tạo biến dữ liệu : db
        BSDBContext db = new BSDBContext();

        // GET: Admin/Login : trang đăng nhập
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Admin/Login/AdminProfile : button xem thông tin về người quản lý
        //Partial View : AdminProfile
        public ActionResult AdminProfile()
        {
            return PartialView();
        }

        //GET : /Admin/Login/AdminInfo : trang xem thông tin người quản lý
        public ActionResult AdminInfo()
        {
            //lấy dữ liệu session
            var model = Session["LoginAdmin"];

            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                if (Session["LoginAdmin"] != null)
                {
                    //so sánh và tìm tên tài khoản
                    var result = db.Admins.SingleOrDefault(x => x.TaiKhoan == model);
                    //trả về dữ liệu tương ứng trong View
                    return View(result);
                }             
            }

            return View();
        }

        //GET : /Admin/Login/Logout :  trang đăng xuất tài khoản người quản lý
        public ActionResult Logout()
        {
            //gán session bằng null
            Session["LoginAdmin"] = null;

            //trả về trang đăng nhập
            return Redirect("/Admin/Login");
        }

        //POST : /Admin/Login/Index : thực hiện việc đăng nhập người quản lý
        [HttpPost]
        public ActionResult Index(LoginModel model)
        {
            //kiểm tra hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //gọi hàm đăng nhập trong AdminProcess và gán dữ liệu trong biến model
                var result = new AdminProcess().Login(model.TaiKhoan, model.MatKhau);
                //Nếu đúng
                if (result == 1)
                {
                    //gán Session["LoginAdmin"] bằng dữ liệu đã đăng nhập
                    Session["LoginAdmin"] = model.TaiKhoan;

                    //trả về trang quản lý
                    return RedirectToAction("Index", "Home");                
                }
                //nếu tài khoản không tồn tại
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại.");
                }
                //nếu nhập sai tài khoản hoặc mật khẩu
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không chính xác");
                }
            }

            return View();
        }
    }
}