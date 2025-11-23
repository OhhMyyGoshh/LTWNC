
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace WebBanSach.Controllers
{
    public class HomeController : Controller
    {
        //Trang chủ chương trình

        //Khởi tạo biến dữ liệu : db
        BSDBContext db = new BSDBContext();

        //GET: /Home/SearchSuggest : trả về danh sách gợi ý autocomplete cho tìm kiếm
        [HttpGet]
        public JsonResult SearchSuggest(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            var suggestions = db.Saches
                .Include("TheLoai")
                .Include("NhaXuatBan")
                .Include("TacGia")
                .Where(s =>
                    s.TenSach.ToLower().Contains(term.ToLower()) ||
                    (s.TacGia != null && s.TacGia.TenTG.ToLower().Contains(term.ToLower()))
                )
                .OrderBy(s => s.TenSach)
                .Take(10)
                .ToList()
                .Select(s => new {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach ?? "Không rõ tên",
                    AnhBia = string.IsNullOrEmpty(s.AnhBia) ? "no-image.png" : s.AnhBia,
                    GiaBan = s.GiaBan ?? 0,
                    TenLoai = (s.TheLoai != null && !string.IsNullOrEmpty(s.TheLoai.TenLoai)) ? s.TheLoai.TenLoai : "Không rõ thể loại",
                    NhaXuatBan = (s.NhaXuatBan != null && !string.IsNullOrEmpty(s.NhaXuatBan.TenNXB)) ? s.NhaXuatBan.TenNXB : "Không rõ NXB",
                    TacGia = (s.TacGia != null && !string.IsNullOrEmpty(s.TacGia.TenTG)) ? s.TacGia.TenTG : "Không rõ tác giả"
                })
                .ToList();
            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }

        //GET : Home/ : trang chủ 
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Home/ShowCategory : hiển thị danh mục phía bên trái trang chủ
        //Parital View : Showcategory
        public ActionResult ShowCategory()
        {
            //gọi hàm xuất danh sách thể loại
            var result = new HomeProcess().ListCategory();

            return PartialView(result);
        }

        public ActionResult ThemesBook(int id)
        {
            var tenloai = new AdminProcess().GetIdCategory(id);
            ViewBag.TenLoai = tenloai.TenLoai;

            var result = new BookProcess().ThemeBook(id);
            return View(result);
        }

        //GET : Show About page
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        //Trang quy định của web
        public ActionResult QuyDinh()
        {
            return View();
        }

        public ActionResult Thanhtoan()
        {
            return View();
        }
        public ActionResult VanChuyen()
        {
            return View();
        }
        public ActionResult DoiTra()
        {
            return View();
        }
        public ActionResult Help()
        {
            return View();
        }

        //GET : /Home/Contact : trang liên hệ, phản hồi của khách hàng
        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Chúng tôi rất hân hạnh lắng nghe ý kiến từ quý khách!";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(LienHe model)
        {
            if (ModelState.IsValid)
            {
                var home = new HomeProcess();
                var lh = new LienHe();

                //gán dữ liệu từ client vào model
                lh.Ten = model.Ten;
                lh.Ho = model.Ho;
                lh.Email = model.Email;
                lh.DienThoai = model.DienThoai;
                lh.NoiDung = model.NoiDung;
                lh.NgayCapNhat = DateTime.Now;

                //gọi hàm lưu thông tin phản hồi từ khách hàng
                var result = home.InsertContact(lh);

                if (result > 0)
                {
                    ViewBag.success = "Đã ghi nhận phản hồi của bạn";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Lỗi ghi nhận");
                }
            }

            return View(model);
        }

        //GET : /Home/SearchResult : trang tìm kiếm sách
        [HttpGet]
        public ActionResult SearchResult(int? page, string key)
        {
            ViewBag.Key = key;

            //phân trang
            int pageNumber = (page ?? 1);
            int pageSize = 6;

            var result = new HomeProcess().Search(key).ToPagedList(pageNumber, pageSize);

            if (result.Count == 0 || key==null || key=="")
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào";
                return View(result);
            }
            ViewBag.ThongBao = "Hiện có " + result.Count + " kết quả ở trang này";

            return View(result);
        }

        //POST : /Home/SearchResult : thực hiện việc tìm kiếm sách
        [HttpPost]
        public ActionResult SearchResult(int? page, FormCollection f)
        {
            //gán từ khóa tìm kiếm được nhập từ client
            string key = f["txtSearch"].ToString();

            ViewBag.Key = key;

            //phân trang
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            
            var result = new HomeProcess().Search(key).ToPagedList(pageNumber, pageSize);

            if (result.Count == 0 || key == null || key == "")
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào";
                return View(result);
            }
            ViewBag.ThongBao = "Hiện có " + result.Count + " kết quả ở trang này";

            return View(result);
        }

    }
}