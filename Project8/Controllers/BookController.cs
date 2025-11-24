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

        //GET : /Book/Details/:id : hiển thị chi tiết thông tin sách + rating
        public ActionResult Details(int? id)
        {
            if (id == null)
                return HttpNotFound();

            var result = new AdminProcess().GetIdBook(id.Value);
            if (result == null)
                return HttpNotFound();

            // Lấy thông tin rating cho sách này
            var ratings = db.DanhGiaSaches.Where(r => r.MaSach == id.Value);

            double avgRating = 0;
            int ratingCount = 0;

            if (ratings.Any())
            {
                ratingCount = ratings.Count();
                avgRating = ratings.Average(r => r.SoSao);
            }

            ViewBag.AvgRating = avgRating;
            ViewBag.RatingCount = ratingCount;

            // Nếu có thông báo từ khi submit rating
            ViewBag.RatingMessage = TempData["RatingMessage"];

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
        [ChildActionOnly]
        public ActionResult RelatedBooks(int? id, int count = 3)
        {
            if (!id.HasValue)
                return new EmptyResult();   // không có id thì không render gì, tránh 500

            var book = db.Saches.Find(id.Value);
            if (book == null)
                return new EmptyResult();

            var related = db.Saches.Where(s => s.MaSach != id.Value &&
                                               (s.MaLoai == book.MaLoai ||
                                                s.MaTG == book.MaTG ||
                                                s.MaNXB == book.MaNXB))
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

        //GET : /Book/All : hiển thị tất cả sách trong db + lọc & sắp xếp
        public ActionResult ShowAllBook(
            int? page,
            int? categoryId,
            int? publisherId,
            int? authorId,
            string priceRange,
            string sortOrder)
        {
            int pageSize = 12;
            int pageNumber = page ?? 1;

            var query = db.Saches.AsQueryable();

            // Lọc theo thể loại
            if (categoryId.HasValue)
            {
                query = query.Where(s => s.MaLoai == categoryId.Value);
            }

            // Lọc theo NXB
            if (publisherId.HasValue)
            {
                query = query.Where(s => s.MaNXB == publisherId.Value);
            }

            // Lọc theo tác giả
            if (authorId.HasValue)
            {
                query = query.Where(s => s.MaTG == authorId.Value);
            }

            // Lọc theo khoảng giá
            if (!string.IsNullOrEmpty(priceRange))
            {
                switch (priceRange)
                {
                    case "1": // dưới 50k
                        query = query.Where(s => s.GiaBan.HasValue && s.GiaBan.Value < 50000);
                        break;
                    case "2": // 50k - 100k
                        query = query.Where(s => s.GiaBan.HasValue && s.GiaBan.Value >= 50000 && s.GiaBan.Value <= 100000);
                        break;
                    case "3": // 100k - 200k
                        query = query.Where(s => s.GiaBan.HasValue && s.GiaBan.Value >= 100000 && s.GiaBan.Value <= 200000);
                        break;
                    case "4": // 200k - 500k
                        query = query.Where(s => s.GiaBan.HasValue && s.GiaBan.Value >= 200000 && s.GiaBan.Value <= 500000);
                        break;
                    case "5": // trên 500k
                        query = query.Where(s => s.GiaBan.HasValue && s.GiaBan.Value > 500000);
                        break;
                }
            }

            // Sắp xếp
            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(s => s.GiaBan);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(s => s.GiaBan);
                    break;
                case "date_desc":
                    query = query.OrderByDescending(s => s.NgayCapNhat);
                    break;
                default:
                    query = query.OrderBy(s => s.TenSach);
                    break;
            }

            // Dropdown lấy từ DB (TheLoai, NXB, TacGia)
            ViewBag.CategoryList = new SelectList(db.TheLoais.OrderBy(t => t.TenLoai), "MaLoai", "TenLoai");
            ViewBag.PublisherList = new SelectList(db.NhaXuatBans.OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            ViewBag.AuthorList = new SelectList(db.TacGias.OrderBy(a => a.TenTG), "MaTG", "TenTG");

            // Giữ lại giá trị filter
            ViewBag.CategoryId = categoryId;
            ViewBag.PublisherId = publisherId;
            ViewBag.AuthorId = authorId;
            ViewBag.PriceRange = priceRange;
            ViewBag.SortOrder = sortOrder;

            var result = query.ToPagedList(pageNumber, pageSize);

            return View(result);
        }

        // POST: /Book/SubmitRating : khách gửi đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRating(int MaSach, int Rating, string Comment)
        {
            // Nếu chưa login thì đẩy về trang chi tiết + báo lỗi
            if (UserController.khachhangstatic == null)
            {
                TempData["RatingMessage"] = "Bạn cần đăng nhập để đánh giá sản phẩm.";
                return RedirectToAction("Details", new { id = MaSach });
            }

            int maKH = UserController.khachhangstatic.MaKH;

            // Kiểm tra đã từng đánh giá sách này chưa
            var existing = db.DanhGiaSaches
                             .FirstOrDefault(x => x.MaSach == MaSach && x.MaKH == maKH);

            if (existing == null)
            {
                var rating = new DanhGiaSach
                {
                    MaSach = MaSach,
                    MaKH = maKH,
                    SoSao = Rating,
                    NoiDung = Comment,
                    NgayDanhGia = DateTime.Now
                };
                db.DanhGiaSaches.Add(rating);
            }
            else
            {
                // Cập nhật lại đánh giá cũ
                existing.SoSao = Rating;
                existing.NoiDung = Comment;
                existing.NgayDanhGia = DateTime.Now;
            }

            db.SaveChanges();

            TempData["RatingMessage"] = "Đánh giá của bạn đã được ghi nhận. Cảm ơn bạn!";
            return RedirectToAction("Details", new { id = MaSach });
        }
    }
}
