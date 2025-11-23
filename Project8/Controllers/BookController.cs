using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using PagedList;
using PagedList.Mvc;

namespace WebBanSach.Controllers
{
    public class BookController : Controller
    {
        BSDBContext db = new BSDBContext();
        // GET: Book
        public ActionResult Index()
        {
            return View();
        }

        //GET : /Book/TopDateBook : hiển thị ra 6 cuốn sách mới cập nhật theo ngày cập nhật
        //Parital View : TopDateBook
        public ActionResult TopDateBook()
        {
            var result = new BookProcess().NewDateBook(6);
            return PartialView(result);
        }

        //GET : /Book/Details/:id : hiển thị chi tiết thông tin sách
        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();
            var result = new AdminProcess().GetIdBook(id.Value);
            if (result == null)
                return HttpNotFound();
            return View(result);
        }

        //GET : /Book/Favorite : hiển thị ra 3 cuốn sách bán chạy theo ngày cập nhật (silde trên cùng)
        //Parital View : FavoriteBook
        public ActionResult FavoriteBook()
        {
            var result = new BookProcess().GetBestSellerBooks(3);
            ViewBag.NoBestSeller = !result.Any();
            return PartialView(result);
        }

        //GET : /Book/RelatedBooks : hiển thị sách liên quan ở trang chi tiết sản phẩm
        //Partial View : RelatedBooks
        public ActionResult RelatedBooks(int id, int count = 3)
        {
            var book = db.Saches.Find(id);
            if (book == null)
                return PartialView(new List<Sach>());

            var related = db.Saches.Where(s => s.MaSach != id &&
                (s.MaLoai == book.MaLoai || s.MaTG == book.MaTG || s.MaNXB == book.MaNXB))
                .OrderByDescending(s => s.NgayCapNhat)
                .Take(count)
                .ToList();
            return PartialView("RelatedBooks", related);
        }

        //GET : /Book/DidYouSee : hiển thị ra 3 cuốn sách giảm dần theo ngày
        //Parital View : DidYouSee
        public ActionResult DidYouSee()
        {
            var result = new BookProcess().TakeBook(3);

            return PartialView(result);
        }

        //GET : /Book/All : hiển thị tất cả sách trong db
        public ActionResult ShowAllBook(int? page)
        {
            //tạo biến số sản phẩm trên trang
            int pageSize = 10;

            //tạo biến số trang
            int pageNumber = (page ?? 1);

            var result = new BookProcess().ShowAllBook().ToPagedList(pageNumber, pageSize);

            return View(result);
        }

    }
}