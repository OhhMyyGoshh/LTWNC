using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using WebBanSach.Models;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;

using Project8.Models.Data;
using Project8.Models.Process;

namespace WebBanSach.Controllers
{
    public class CartController : Controller
    {
        //khởi tạo dữ liệu
        BSDBContext db = new BSDBContext();

        //tạo 1 chuỗi hằng để gán session
        private const string CartSession = "CartSession";

        // GET: Cart/ : trang giỏ hàng
        public ActionResult Index()
        {
            var cart = Session[CartSession];
            var list = new List<CartModel>();
            var sl = 0;
            decimal? total = 0;

            if (cart != null)
            {
                list = (List<CartModel>)cart;
                sl = list.Sum(x => x.Quantity);
                total = list.Sum(x => x.Total);
            }

            ViewBag.Quantity = sl;
            ViewBag.Total = total;
            ViewBag.CartError = TempData["CartError"];   // thông báo lỗi nếu có

            return View(list);
        }

        //GET : /Cart/CartHeader : đếm số sản phẩm trong giỏ hàng
        //PartialView : CartHeader
        public ActionResult CartHeader()
        {
            var cart = Session[CartSession];
            var list = new List<CartModel>();
            if (cart != null)
            {
                list = (List<CartModel>)cart;
            }

            return PartialView(list);
        }

        //Xóa 1 sản phẩm trong giỏ hàng
        public JsonResult Delete(int id)
        {
            var sessionCart = (List<CartModel>)Session[CartSession];
            //xóa những giá trị mà có mã sách giống với id
            sessionCart.RemoveAll(x => x.sach.MaSach == id);
            //gán lại giá trị cho session
            Session[CartSession] = sessionCart;

            return Json(new
            {
                status = true
            });
        }

        //Xóa tất cả các sản phẩm trong giỏ hàng
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        //Cập nhật giỏ hàng
        public JsonResult Update(string cartModel)
        {
            //tạo 1 đối tượng dạng json
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartModel>>(cartModel);

            //ép kiểu từ session
            var sessionCart = (List<CartModel>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.Single(x => x.sach.MaSach == item.sach.MaSach);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            //cập nhật lại session
            Session[CartSession] = sessionCart;

            return Json(new
            {
                status = true
            });
        }

        //GET : /Cart/AddItem/?id=?&quantity=1 : thêm sản phẩm vào giỏ hàng
        public ActionResult AddItem(int id, int quantity)
        {
            // Lấy thông tin sách
            var sach = new AdminProcess().GetIdBook(id);
            if (sach == null)
            {
                TempData["CartError"] = "Không tìm thấy sách.";
                return RedirectToAction("Index");
            }

            int soTon = sach.SoLuongTon ?? 0;
            if (soTon <= 0)
            {
                TempData["CartError"] = "Sản phẩm đã hết hàng, không thể thêm vào giỏ.";
                return RedirectToAction("Details", "Book", new { id = id });
            }

            // Lấy giỏ hàng từ session
            var cart = Session[CartSession] as List<CartModel> ?? new List<CartModel>();

            var existingItem = cart.FirstOrDefault(x => x.sach.MaSach == id);
            int currentQty = existingItem != null ? existingItem.Quantity : 0;

            if (currentQty + quantity > soTon)
            {
                TempData["CartError"] = "Sản phẩm chỉ còn " + soTon + " bản, bạn không thể thêm " + quantity + " nữa.";
                return RedirectToAction("Details", "Book", new { id = id });
            }

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var item = new CartModel
                {
                    sach = sach,
                    Quantity = quantity
                };
                cart.Add(item);
            }

            // Gán lại session
            Session[CartSession] = cart;

            return RedirectToAction("Index");
        }

        //Áp dụng voucher
        [HttpPost]
        public JsonResult ApplyVoucher(string maCode, decimal tongTien)
        {
            var voucherProcess = new VoucherProcess();
            var result = voucherProcess.ApplyVoucher(maCode, tongTien);

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                maVoucher = result.MaVoucher,
                giamGia = result.GiamGia,
                tongTienSauGiam = result.TongTienSauGiam
            });
        }

        //Thông tin khách hàng trên trang Payment
        [HttpGet]
        [ChildActionOnly]
        public PartialViewResult UserInfo()
        {
            var model = Session["User"];
            if (model == null)
            {
                return PartialView();
            }

            if (ModelState.IsValid)
            {
                var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == model);
                return PartialView(result);
            }

            return PartialView();
        }

        //GET: /Cart/Payment : trang thanh toán
        [HttpGet]
        public ActionResult Payment()
        {
            //kiểm tra đăng nhập
            if (Session["User"] == null || Session["User"].ToString() == "")
            {
                return RedirectToAction("LoginPage", "User");
            }

            if (UserController.khachhangstatic.TrangThai == false)
            {
                return RedirectToAction("ThongBaoKichHoat", "User");
            }

            var cart = Session[CartSession];
            var list = new List<CartModel>();
            var sl = 0;
            decimal? total = 0;

            if (cart != null)
            {
                list = (List<CartModel>)cart;
                sl = list.Sum(x => x.Quantity);
                total = list.Sum(x => x.Total);
            }

            ViewBag.Quantity = sl;
            ViewBag.Total = total;
            ViewBag.CartError = TempData["CartError"];

            return View(list);
        }

        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //POST: /Cart/Payment : tạo đơn đặt hàng
        [HttpPost]
        public ActionResult Payment(int MaKH, FormCollection f)
        {
            // Phương thức thanh toán: 1 = COD, 2 = MoMo (hoặc thêm sau)
            int PMethod = 1;
            int tmp;
            if (int.TryParse(f["PaymentMethod"], out tmp))
            {
                PMethod = tmp;
            }

            // Mã voucher đã áp dụng (nếu có)
            int maVoucher = 0;
            int.TryParse(f["MaVoucher"], out maVoucher);

            // Lấy giỏ hàng
            var cart = Session[CartSession] as List<CartModel>;
            if (cart == null || !cart.Any())
            {
                TempData["CartError"] = "Giỏ hàng trống.";
                return RedirectToAction("Index");
            }

            // ===== CHECK TỒN KHO TRƯỚC KHI TẠO ĐƠN (KHÔNG TRỪ TỒN Ở ĐÂY) =====
            foreach (var item in cart)
            {
                var sachDb = db.Saches.Find(item.sach.MaSach);
                if (sachDb == null)
                {
                    TempData["CartError"] = "Không tìm thấy sản phẩm trong hệ thống.";
                    return RedirectToAction("Index");
                }

                int soTon = sachDb.SoLuongTon ?? 0;
                if (soTon <= 0)
                {
                    TempData["CartError"] = "Sản phẩm '" + sachDb.TenSach + "' đã hết hàng, vui lòng xoá khỏi giỏ.";
                    return RedirectToAction("Index");
                }

                if (item.Quantity > soTon)
                {
                    TempData["CartError"] = "Sản phẩm '" + sachDb.TenSach + "' chỉ còn " + soTon + " bản, vui lòng điều chỉnh số lượng.";
                    return RedirectToAction("Index");
                }
            }
            // ===================================================================

            // ==== LẤY THÔNG TIN NGƯỜI NHẬN TỪ FORM ====
            string tenNguoiNhan = f["TenKH"];
            string emailNguoiNhan = f["Email"];
            string diaChiNguoiNhan = f["DiaChi"];
            string dienThoaiNguoiNhan = f["DienThoai"];
           

            var kh = db.KhachHangs.Find(MaKH);
            if (kh != null)
            {
                if (string.IsNullOrWhiteSpace(tenNguoiNhan)) tenNguoiNhan = kh.TenKH;
                if (string.IsNullOrWhiteSpace(emailNguoiNhan)) emailNguoiNhan = kh.Email;
                if (string.IsNullOrWhiteSpace(diaChiNguoiNhan)) diaChiNguoiNhan = kh.DiaChi;
                if (string.IsNullOrWhiteSpace(dienThoaiNguoiNhan)) dienThoaiNguoiNhan = kh.DienThoai;
            }
            var order = new DonDatHang
            {
                NgayDat = DateTime.Now,
                NgayGiao = DateTime.Now.AddDays(3),
                TinhTrang = true,        // tuỳ anh định nghĩa
                MaKH = MaKH,
                GhiChu = f["GhiChu"],
                ThanhToan = PMethod,     // 1: COD, 2: MoMo
                TrangThaiDonHang = 0,           // Chờ xác nhận
                MaVoucher = null,
                GiamGia = 0,

                // *** LẤY THÔNG TIN NGƯỜI NHẬN TỪ FORM ***
                TenNguoiNhan = f["TenKH"],      // name="TenKH" trên view
                EmailNguoiNhan = f["Email"],     // name="Email"
                DiaChiNhan = f["DiaChi"],     // name="DiaChi"
                DienThoaiNhan = f["DienThoai"]   // name="DienThoai"
            };


            try
            {
                // Tổng tiền trước giảm
                decimal? totalNullable = cart.Sum(x => x.Total);
                if (totalNullable == null) totalNullable = 0m;
                decimal total = totalNullable.Value;

                // Áp dụng voucher nếu có
                if (maVoucher > 0)
                {
                    try
                    {
                        var voucher = db.Vouchers.Find(maVoucher);
                        if (voucher != null)
                        {
                            var voucherProcess = new VoucherProcess();
                            var voucherResult = voucherProcess.ApplyVoucher(
                                voucher.MaCode,
                                total,
                                MaKH
                            );

                            if (voucherResult.Success)
                            {
                                order.MaVoucher = maVoucher;
                                // ép kiểu rõ ràng để tránh lỗi decimal?/decimal
                                order.GiamGia = (decimal)voucherResult.GiamGia;
                                // nếu cần có thể dùng voucherResult.TongTienSauGiam để hiển thị
                                total = voucherResult.TongTienSauGiam;
                            }
                        }
                    }
                    catch
                    {
                        // Nếu voucher lỗi, vẫn cho đặt hàng bình thường
                    }
                }

                // Tạo đơn hàng
                var result1 = new OrderProcess().Insert(order);

                // Lưu chi tiết đơn hàng
                var result2 = new OderDetailProcess();
                foreach (var item in cart)
                {
                    var orderDetail = new ChiTietDDH
                    {
                        MaSach = item.sach.MaSach,
                        MaDDH = result1,
                        SoLuong = item.Quantity,
                        DonGia = item.sach.GiaBan
                    };
                    result2.Insert(orderDetail);
                }

                // KHÔNG TRỪ TỒN KHO Ở ĐÂY
                // Tồn kho chỉ trừ khi ADMIN hoặc KHÁCH HÀNG xác nhận ĐÃ GIAO (status = 3)

                // Đánh dấu voucher đã sử dụng
                if (maVoucher > 0 && order.MaVoucher != null)
                {
                    try
                    {
                        new VoucherProcess().UseVoucher(maVoucher, MaKH, result1);
                    }
                    catch
                    {
                        // Log lỗi nhưng không ảnh hưởng đơn hàng
                    }
                }

                // Xoá giỏ hàng sau khi đặt xong
                Session[CartSession] = null;

                // === ĐIỀU HƯỚNG THEO PHƯƠNG THỨC THANH TOÁN ===
                if (PMethod == 2) // MoMo
                {
                    long amount = (long)Math.Round(total, 0, MidpointRounding.AwayFromZero);
                    return RedirectToAction("MoMoPayment", new { orderId = result1, amount = amount });
                }

                // COD hoặc các phương thức khác => vào trang Success như cũ
                return RedirectToAction("Success");

            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                System.Diagnostics.Debug.WriteLine("Lỗi đặt hàng: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }

                ViewBag.ErrorMessage = ex.Message;
                return Redirect("/Cart/Error");
            }

        }
        [HttpGet]
        public ActionResult MomoPayment(int orderId, long amount)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            ViewBag.TransferContent = "BOOK-" + orderId; // nội dung chuyển tiền

            return View();
        }


        public ActionResult Success()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult TrackingOder(int? status)
        {
            var query = db.DonDatHangs
                .Include("ChiTietDDHs.Sach")
                .Include("KhachHang")
                .Include("Voucher")
                .Where(p => p.MaKH == UserController.khachhangstatic.MaKH);

            // Lọc theo trạng thái nếu có
            if (status.HasValue)
            {
                query = query.Where(x => x.TrangThaiDonHang == status.Value);
            }

            var donDatHang = query.OrderByDescending(x => x.MaDDH).ToList();
            ViewBag.CurrentStatus = status;
            return View(donDatHang);
        }

        public ActionResult TrackingOderDetails(int id)
        {
            var order = db.DonDatHangs
                .Include("ChiTietDDHs.Sach")
                .Include("KhachHang")
                .Include("Voucher")
                .FirstOrDefault(x => x.MaDDH == id);

            if (order == null)
            {
                return RedirectToAction("TrackingOder");
            }

            return View(order);
        }

        //POST : /Cart/CancelOrder : hủy đơn hàng
        [HttpPost]
        public JsonResult CancelOrder(int orderId)
        {
            try
            {
                var order = db.DonDatHangs.Find(orderId);

                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Chỉ cho phép hủy khi đơn hàng đang chờ xác nhận
                if (order.TrangThaiDonHang != 0)
                {
                    return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xác nhận" });
                }

                // Kiểm tra quyền sở hữu
                if (order.MaKH != UserController.khachhangstatic.MaKH)
                {
                    return Json(new { success = false, message = "Bạn không có quyền hủy đơn hàng này" });
                }

                // Cập nhật trạng thái thành "Đã hủy" (4)
                order.TrangThaiDonHang = 4;
                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Đã hủy đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        // POST: /Cart/ConfirmReceived : khách xác nhận đã nhận hàng
        [HttpPost]
        public JsonResult ConfirmReceived(int orderId)
        {
            try
            {
                var order = db.DonDatHangs.Find(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra quyền sở hữu
                if (order.MaKH != UserController.khachhangstatic.MaKH)
                {
                    return Json(new { success = false, message = "Bạn không có quyền xác nhận đơn hàng này" });
                }

                // Chỉ cho phép xác nhận khi đơn đang giao
                if (order.TrangThaiDonHang != 2)
                {
                    return Json(new { success = false, message = "Chỉ có thể xác nhận đơn hàng đang giao" });
                }

                // Dùng cùng logic với Admin để cập nhật trạng thái + trừ tồn kho
                var process = new AdminProcess();
                string msg;
                bool ok = process.UpdateOrderStatusAndStock(orderId, 3, out msg); // 3 = Đã giao

                if (!ok)
                {
                    return Json(new { success = false, message = msg });
                }

                return Json(new { success = true, message = "Cảm ơn bạn! Đơn hàng đã được xác nhận ĐÃ GIAO." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi: " + ex.Message });
            }
        }

        public JsonResult loadOrder()
        {
            db.Configuration.ProxyCreationEnabled = false;
            var donDatHang = db.DonDatHangs.ToList();

            return Json(new { data = donDatHang }
                , JsonRequestBehavior.AllowGet);
        }
    }
}
