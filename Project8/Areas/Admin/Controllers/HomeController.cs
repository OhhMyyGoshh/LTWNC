using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using System.IO;
using Project8.Areas.Admin.Code;
using System.Data.Entity;

namespace WebBanSach.Areas.Admin.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        //Trang quản lý

        //Khởi tạo biến dữ liệu : db
        BSDBContext db = new BSDBContext();

        // GET: Admin/Home : trang chủ Admin
        public ActionResult Index()
        {
            return View();
        }

        #region Sản phẩm

        //GET : Admin/Home/ShowListBook : Trang quản lý sách
        [HttpGet]
        public ActionResult ShowListBook()
        {
            //Gọi hàm ListAllBook và truyền vào model trả về View
            var model = new AdminProcess().ListAllBook();

            return View(model);
        }

        //GET : Admin/Home/AddBook : Trang thêm sách mới
        public ActionResult AddBook()
        {
            //lấy mã mà hiển thị tên
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG");

            return View();
        }

        //POST : Admin/Home/AddBook : thực hiện thêm sách
        [HttpPost]
        public ActionResult AddBook(Sach sach, HttpPostedFileBase fileUpload)
        {
            //lấy mã mà hiển thị tên
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB");
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG");

            //kiểm tra việc upload ảnh
            if (fileUpload == null)
            {
                ViewBag.Alert = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                //kiểm tra dữ liệu db có hợp lệ?
                if (ModelState.IsValid)
                {
                    //lấy file đường dẫn
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    //chuyển file đường dẫn và biên dịch vào /images
                    var path = Path.Combine(Server.MapPath("/images"), fileName);

                    //kiểm tra đường dẫn ảnh có tồn tại?
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Alert = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }

                    //thực hiện việc lưu đường dẫn ảnh vào link ảnh bìa
                    sach.AnhBia = fileName;
                    //thực hiện lưu vào db
                    var result = new AdminProcess().InsertBook(sach);
                    if (result > 0)
                    {
                        ViewBag.Success = "Thêm mới thành công";
                        //xóa trạng thái để thêm mới
                        ModelState.Clear();
                    }
                    else
                    {
                        ModelState.AddModelError("", "thêm không thành công.");
                    }
                }
            }

            return View();
        }

        //GET : Admin/Home/DetailsBook/:id : Trang xem chi tiết 1 cuốn sách
        [HttpGet]
        public ActionResult DetailsBook(int id)
        {
            //gọi hàm lấy id sách và truyền vào View
            var sach = new AdminProcess().GetIdBook(id);

            return View(sach);
        }

        public ActionResult UpdateBook(int id)
        {
            //gọi hàm lấy mã sách
            var sach = new AdminProcess().GetIdBook(id);

            //thực hiện việc lấy mã nhưng hiển thị tên và đúng tại mã đang chỉ định và gán vào ViewBag
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai", sach.MaLoai);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG", sach.MaTG);

            return View(sach);
        }

        //POST : /Admin/Home/UpdateBook : thực hiện việc cập nhật sách
        //Tương tự như thêm sách
        [HttpPost]
        public ActionResult UpdateBook(Sach sach, HttpPostedFileBase fileUpload)
        {
            //thực hiện việc lấy mã nhưng hiển thị tên ngay đúng mã đã chọn và gán vào ViewBag
            ViewBag.MaLoai = new SelectList(db.TheLoais.ToList().OrderBy(x => x.TenLoai), "MaLoai", "TenLoai", sach.MaLoai);
            ViewBag.MaNXB = new SelectList(db.NhaXuatBans.ToList().OrderBy(x => x.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);
            ViewBag.MaTG = new SelectList(db.TacGias.ToList().OrderBy(x => x.TenTG), "MaTG", "TenTG", sach.MaTG);

            //Nếu không thay đổi ảnh bìa thì làm
            if (fileUpload == null)
            {
                //kiểm tra hợp lệ dữ liệu
                if (ModelState.IsValid)
                {
                    //gọi hàm UpdateBook cho việc cập nhật sách
                    var result = new AdminProcess().UpdateBook(sach);

                    if (result == 1)
                    {
                        ViewBag.Success = "Cập nhật thành công";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Cập nhật không thành công.");
                    }
                }
            }
            //nếu thay đổi ảnh bìa thì làm
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("/images"), fileName);

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Alert = "Hình ảnh đã tồn tại";
                    }
                    else
                    {
                        fileUpload.SaveAs(path);
                    }

                    sach.AnhBia = fileName;
                    var result = new AdminProcess().UpdateBook(sach);
                    if (result == 1)
                    {
                        ViewBag.Success = "Cập nhật thành công";
                    }
                    else
                    {
                        ModelState.AddModelError("", "cập nhật không thành công.");
                    }
                }
            }

            return View(sach);
        }

        //DELETE : Admin/Home/DeleteBook/:id : thực hiện xóa 1 cuốn sách
        [HttpDelete]
        public ActionResult DeleteBook(int id)
        {
            //gọi hàm DeleteBook để thực hiện xóa
            new AdminProcess().DeleteBook(id);

            //trả về trang quản lý sách
            return RedirectToAction("ShowListBook");
        }

        //Category

        //GET : /Admin/Home/ShowListCategory : trang quản lý thể loại
        [HttpGet]
        public ActionResult ShowListCategory()
        {
            //gọi hàm ListAllCategory để hiện những thể loại trong db
            var model = new AdminProcess().ListAllCategory();

            return View(model);
        }

        //GET : Admin/Home/AddCategory : trang thêm thể loại
        [HttpGet]
        public ActionResult AddCategory()
        {
            return View();
        }

        //POST : Admin/Home/AddCategory/:model : thực hiện việc thêm thể loại vào db
        [HttpPost]
        public ActionResult AddCategory(TheLoai model)
        {
            //kiểm tra dữ liệu hợp lệ
            if (ModelState.IsValid)
            {
                //khởi tao biến admin trong WebBanSach.Models.Process
                var admin = new AdminProcess();

                //khởi tạo biến thuộc đối tượng thể loại trong db
                var tl = new TheLoai();

                //gán thuộc tính tên thể loại
                tl.TenLoai = model.TenLoai;

                //gọi hàm thêm thể loại (InsertCategory) trong biến admin
                var result = admin.InsertCategory(tl);

                //kiểm tra hàm
                if (result > 0)
                {
                    ViewBag.Success = "Thêm mới thành công";
                    //xóa trạng thái
                    ModelState.Clear();

                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công.");
                }
            }

            return View(model);
        }

        //GET : Admin/Home/UpdateCategory/:id : trang cập nhật thể loại
        [HttpGet]
        public ActionResult UpdateCategory(int id)
        {
            //gọi hàm lấy mã thể loại
            var tl = new AdminProcess().GetIdCategory(id);

            //trả về dữ liệu View tương ứng
            return View(tl);
        }

        //POST : /Admin/Home/UpdateCategory/:id : thực hiện việc cập nhật thể loại
        [HttpPost]
        public ActionResult UpdateCategory(TheLoai tl)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //gọi hàm cập nhật thể loại
                var result = admin.UpdateCategory(tl);

                //thực hiện kiểm tra
                if (result == 1)
                {
                    return RedirectToAction("ShowListCategory");
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }

            return View(tl);
        }

        //DELETE : /Admin/Home/DeleteCategory:id : thực hiện xóa thể loại
        [HttpDelete]
        public ActionResult DeleteCategory(int id)
        {
            // gọi hàm xóa thể loại
            new AdminProcess().DeleteCategory(id);

            //trả về trang quản lý thể loại
            return RedirectToAction("ShowListCategory");
        }

        //Author

        //GET : /Admin/Home/ShowListAuthor : trang quản lý tác giả
        [HttpGet]
        public ActionResult ShowListAuthor()
        {
            //gọi hàm xuất danh sách tác giả trong db
            var model = new AdminProcess().ListAllAuthor();

            //trả về View tương ứng
            return View(model);
        }

        //GET : /Admin/Home/AddAuthor : trang thêm tác giả
        public ActionResult AddAuthor()
        {
            return View();
        }

        //POST : /Admin/Home/AddAuthor/:model : thực hiện việc thêm tác giả
        [HttpPost]
        public ActionResult AddAuthor(TacGia model)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //khởi tạo đối tượng tg
                var tg = new TacGia();

                //gán dữ liệu
                tg.TenTG = model.TenTG;
                tg.QueQuan = model.QueQuan;
                tg.NgaySinh = model.NgaySinh;
                tg.NgayMat = model.NgayMat;
                tg.TieuSu = model.TieuSu;

                //gọi hàm thêm tác giả
                var result = admin.InsertAuthor(tg);

                //kiểm tra hàm
                if (result > 0)
                {
                    ViewBag.Success = "Thêm mới thành công";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công.");
                }
            }

            return View(model);
        }

        //GET : /Admin/Home/UpdateAuthor/:id : trang thêm tác giả 
        [HttpGet]
        public ActionResult UpdateAuthor(int id)
        {
            //gọi hàm lấy mã tác giả
            var tg = new AdminProcess().GetIdAuthor(id);

            return View(tg);
        }

        //POST : /Admin/Home/UpdateAuthor/:id : thực hiện việc thêm tác giả
        [HttpPost]
        public ActionResult UpdateAuthor(TacGia tg)
        {
            //kiểm tra hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //gọi hàm cập nhật tác giả
                var result = admin.UpdateAuthor(tg);
                //thực hiển kiểm tra
                if (result == 1)
                {
                    ViewBag.Success = "Cập nhật thành công";
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }

            return View(tg);
        }

        //DELETE : /Admin/Home/DeleteAuthor/:id : thực hiện xóa tác giả
        [HttpDelete]
        public ActionResult DeleteAuthor(int id)
        {
            //gọi hàm xóa tác giả
            new AdminProcess().DeleteAuthor(id);

            return RedirectToAction("ShowListAuthor");
        }

        //Publish

        //GET : /Admin/Home/ShowListPublish : trang quản lý nhà xuất bản
        [HttpGet]
        public ActionResult ShowListPublish()
        {
            //gọi hàm xuất danh sách nhà xuất bản
            var model = new AdminProcess().ListAllPublish();

            return View(model);
        }

        //GET : /Admin/Home/AddPublish : trang quản lý nhà xuất bản
        public ActionResult AddPublish()
        {
            return View();
        }

        //POST : /Admin/Home/AddPublish/:model : thực hiện việc thêm nhà xuất bản
        [HttpPost]
        public ActionResult AddPublish(NhaXuatBan model)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //khởi tạo object(đối tượng) nhà xuất bản
                var nxb = new NhaXuatBan();

                //gán dữ liệu
                nxb.TenNXB = model.TenNXB;
                nxb.DiaChi = model.DiaChi;
                nxb.DienThoai = model.DienThoai;

                //gọi hàm thêm nhà xuất bản
                var result = admin.InsertPublish(nxb);
                //kiểm tra hàm
                if (result > 0)
                {
                    ViewBag.Success = "Thêm mới thành công";
                    ModelState.Clear();
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "Thêm không thành công.");
                }
            }

            return View(model);
        }

        //GET : /Admin/Home/UpdatePublish/:id : trang thêm nhà xuất bản
        [HttpGet]
        public ActionResult UpdatePublish(int id)
        {
            //gọi hàm lấy mã nhà xuất bản
            var nxb = new AdminProcess().GetIdPublish(id);

            return View(nxb);
        }

        //GET : /Admin/Home/UpdatePublish/:id : thực hiện thêm nhà xuất bản
        [HttpPost]
        public ActionResult UpdatePublish(NhaXuatBan nxb)
        {
            //kiểm tra tính hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                //khởi tạo biến admin
                var admin = new AdminProcess();

                //gọi hàm cập nhật nhà xuất bản
                var result = admin.UpdatePublish(nxb);
                //kiểm tra hàm
                if (result == 1)
                {
                    ViewBag.Success = "Cập nhật nhật thành công";
                }
                else
                {
                    ModelState.AddModelError("", "Cập nhật không thành công.");
                }
            }

            return View(nxb);
        }

        //DELETE : Admin/Home/DeletePublish/:id : thực hiện xóa nhà xuất bản
        [HttpDelete]
        public ActionResult DeletePublish(int id)
        {
            //gọi hàm xóa hàm xuất bản
            new AdminProcess().DeletePublish(id);

            //trả về trang quản lý nhà xuất bản
            return RedirectToAction("ShowListPublish");
        }

        #endregion

        #region Phản hồi

        //Contact/Feedback : Liên hệ / phản hồi khách hàng

        [HttpGet]
        //GET : Admin/Home/FeedBack : xem danh sách thông báo phản hồi
        public ActionResult FeedBack()
        {
            var result = new AdminProcess().ShowListContact();

            return View(result);
        }

        //GET : Admin/Home/FeedDetail/:id : xem nội dung phản hồi khách hàng
        public ActionResult FeedDetail(int id)
        {
            var result = new AdminProcess().GetIdContact(id);

            return View(result);
        }

        //DELETE : Admin/Home/DeleteFeedBack/:id : xóa thông tin phản hồi khách hàng
        [HttpDelete]
        public ActionResult DeleteFeedBack(int id)
        {
            new AdminProcess().deleteContact(id);

            return RedirectToAction("FeedBack");
        }

        #endregion

        #region Người dùng

        //GET : /Admin/Home/ShowUser : trang quản lý người dùng
        public ActionResult ShowUser()
        {
            var result = new AdminProcess().ListUser();

            return View(result);
        }

        //GET : /Admin/Home/DetailsUser/:id : trang xem chi tiết người dùng
        public ActionResult DetailsUser(int id)
        {
            var result = new AdminProcess().GetIdCustomer(id);

            return View(result);
        }

        [HttpPost]
        public JsonResult ChangeUserStatus(int id)
        {
            var process = new AdminProcess();
            var status = process.ChangeStatusUser(id);

            return Json(new
            {
                success = true,
                status = status
            }, JsonRequestBehavior.AllowGet);
        }

        //DELETE : Admin/Home/DeleteUser/:id : xóa thông tin người dùng
        [HttpDelete]
        public ActionResult DeleteUser(int id)
        {
            new AdminProcess().DeleteUser(id);

            return RedirectToAction("ShowUser");
        }

        #endregion

        #region Đơn đặt hàng

        //GET : Admin/Home/Order : trang quản lý đơn đặt hàng
        public ActionResult Order(DateTime? fromDate, DateTime? toDate, int? status)
        {
            var query = db.DonDatHangs
                .Include(x => x.KhachHang)
                .Include(x => x.ChiTietDDHs)
                .Include(x => x.Voucher)
                .AsQueryable();

            // Lọc theo ngày
            if (fromDate.HasValue)
            {
                var startDate = fromDate.Value.Date;
                query = query.Where(x => x.NgayDat >= startDate);
            }
            if (toDate.HasValue)
            {
                var endDate = toDate.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.NgayDat <= endDate);
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                query = query.Where(x => x.TrangThaiDonHang == status.Value);
            }

            var result = query.OrderByDescending(x => x.MaDDH).ToList();

            ViewBag.FromDate = fromDate.HasValue ? fromDate.Value.ToString("yyyy-MM-dd") : null;
            ViewBag.ToDate = toDate.HasValue ? toDate.Value.ToString("yyyy-MM-dd") : null;
            ViewBag.Status = status;
            ViewBag.TotalOrders = result.Count();
            ViewBag.HasFilter = fromDate.HasValue || toDate.HasValue || status.HasValue;

            return View(result);
        }

        //GET : /Admin/Home/DetailsOrder : trang xem chi tiết đơn hàng
        public ActionResult DetailsOrder(int id)
        {
            var result = new OderDetailProcess().ListDetail(id);

            return View(result);
        }

        //POST : /Admin/Home/UpdateOrderStatus : cập nhật trạng thái đơn hàng (gọi AdminProcess để trừ tồn)
        [HttpPost]
        public JsonResult UpdateOrderStatus(int orderId, int status)
        {
            try
            {
                var process = new AdminProcess();
                string message;
                bool ok = process.UpdateOrderStatusAndStock(orderId, status, out message);

                if (!ok)
                {
                    return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Voucher

        // GET: /Admin/Home/Voucher
        [HttpGet]
        public ActionResult Voucher()
        {
            var model = new AdminProcess().ListAllVoucher();
            return View(model);
        }

        // GET: /Admin/Home/AddVoucher
        [HttpGet]
        public ActionResult AddVoucher()
        {
            return View();
        }

        // POST: /Admin/Home/AddVoucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddVoucher(Voucher model)
        {
            if (ModelState.IsValid)
            {
                var process = new AdminProcess();

                if (!model.NgayBatDau.HasValue)
                {
                    model.NgayBatDau = DateTime.Now;
                }

                int id = process.InsertVoucher(model);
                if (id > 0)
                {
                    TempData["Success"] = "Thêm voucher thành công.";
                    return RedirectToAction("Voucher");
                }

                ModelState.AddModelError("", "Thêm voucher không thành công.");
            }

            return View(model);
        }

        // GET: /Admin/Home/UpdateVoucher/5
        [HttpGet]
        public ActionResult UpdateVoucher(int id)
        {
            var vc = new AdminProcess().GetIdVoucher(id);
            if (vc == null) return HttpNotFound();
            return View(vc);
        }

        // POST: /Admin/Home/UpdateVoucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateVoucher(Voucher model)
        {
            if (ModelState.IsValid)
            {
                var process = new AdminProcess();
                int result = process.UpdateVoucher(model);
                if (result == 1)
                {
                    TempData["Success"] = "Cập nhật voucher thành công.";
                    return RedirectToAction("Voucher");
                }

                ModelState.AddModelError("", "Cập nhật voucher không thành công.");
            }

            return View(model);
        }

        // DELETE: /Admin/Home/DeleteVoucher/5
        [HttpDelete]
        public ActionResult DeleteVoucher(int id)
        {
            new AdminProcess().DeleteVoucher(id);
            return RedirectToAction("Voucher");
        }

        #endregion

        #region Đánh giá (Rating & Review)

        // GET: /Admin/Home/Review
        [HttpGet]
        public ActionResult Review(bool? daDuyet)
        {
            var reviews = db.DanhGiaSaches
                .Include("Sach")
                .Include("KhachHang")
                .AsQueryable();

            if (daDuyet.HasValue)
            {
                reviews = reviews.Where(r => r.TrangThaiDuyet == daDuyet.Value);
            }

            var list = reviews
                .OrderByDescending(r => r.NgayDanhGia)
                .ToList();

            ViewBag.DaDuyet = daDuyet;
            ViewBag.TotalReviews = list.Count();

            return View(list);
        }

        // POST: /Admin/Home/ToggleReviewStatus/:id
        [HttpPost]
        public JsonResult ToggleReviewStatus(int id)
        {
            var review = db.DanhGiaSaches.Find(id);
            if (review == null)
                return Json(new { success = false, message = "Không tìm thấy đánh giá" });

            review.TrangThaiDuyet = !review.TrangThaiDuyet;
            db.Entry(review).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                message = review.TrangThaiDuyet ? "Đã duyệt đánh giá" : "Đã ẩn đánh giá"
            });
        }

        // DELETE: /Admin/Home/DeleteReview/:id
        [HttpDelete]
        public ActionResult DeleteReview(int id)
        {
            var review = db.DanhGiaSaches.Find(id);
            if (review != null)
            {
                db.DanhGiaSaches.Remove(review);
                db.SaveChanges();
            }
            return RedirectToAction("Review");
        }

        #endregion
        #region Vòng quay voucher

        [HttpGet]
        public ActionResult LuckyWheelConfig()
        {
            var segments = db.VongQuayVouchers
                             .Include("Voucher")
                             .OrderBy(x => x.Id)
                             .ToList();

            ViewBag.VoucherList = new SelectList(
                db.Vouchers.OrderBy(v => v.TenVoucher),
                "MaVoucher",
                "TenVoucher"
            );

            return View(segments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddWheelSegment(int? MaVoucher, string TenO, int TiLeTrung = 1, string MauHex = "#f1c40f")
        {
            if (string.IsNullOrWhiteSpace(TenO) && MaVoucher.HasValue)
            {
                var vc = db.Vouchers.Find(MaVoucher.Value);
                if (vc != null)
                {
                    TenO = "Voucher " + vc.TenVoucher;
                }
            }

            var seg = new VongQuayVoucher
            {
                MaVoucher = MaVoucher,
                TenO = string.IsNullOrWhiteSpace(TenO) ? "Trượt" : TenO,
                TiLeTrung = TiLeTrung <= 0 ? 1 : TiLeTrung,
                MauHex = string.IsNullOrWhiteSpace(MauHex) ? "#f1c40f" : MauHex,
                HoatDong = true
            };

            db.VongQuayVouchers.Add(seg);
            db.SaveChanges();

            return RedirectToAction("LuckyWheelConfig");
        }

        [HttpPost]
        public JsonResult ToggleWheelSegment(int id)
        {
            var seg = db.VongQuayVouchers.Find(id);
            if (seg == null)
                return Json(new { success = false, message = "Không tìm thấy ô" });

            seg.HoatDong = !seg.HoatDong;
            db.SaveChanges();

            return Json(new
            {
                success = true,
                active = seg.HoatDong,
                message = seg.HoatDong ? "Đã bật ô quay" : "Đã tắt ô quay"
            });
        }

        [HttpDelete]
        public ActionResult DeleteWheelSegment(int id)
        {
            var seg = db.VongQuayVouchers.Find(id);
            if (seg == null)
            {
                return HttpNotFound();
            }

            // Xóa hết log lượt quay liên quan ô này
            var logs = db.LuotQuayVouchers
                         .Where(l => l.VongQuayVoucherId == seg.Id)
                         .ToList();

            if (logs.Any())
            {
                db.LuotQuayVouchers.RemoveRange(logs);
            }

            // Sau đó xóa ô vòng quay
            db.VongQuayVouchers.Remove(seg);
            db.SaveChanges();

            return RedirectToAction("LuckyWheelConfig");
        }


        #endregion

    }
}
