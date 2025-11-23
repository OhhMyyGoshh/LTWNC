using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using WebBanSach.Models;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            return View(list);
        }

        //GET : /Cart/CartHeader : đếm sổ sản phẩm trong giỏ hàng
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
            //lấy mã sách và gán đối tượng
            var sach = new AdminProcess().GetIdBook(id);

            //lấy giỏ hàng từ session
            var cart = Session[CartSession];

            //nếu đã có sản phẩm trong giỏ hàng
            if (cart != null)
            {
                var list = (List<CartModel>)cart;
                if (list.Exists(x => x.sach.MaSach == id))
                {

                    foreach (var item in list)
                    {
                        if (item.sach.MaSach == id)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item
                    var item = new CartModel();
                    item.sach = sach;
                    item.Quantity = quantity;
                    list.Add(item);
                }

                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới giỏ hàng
                var item = new CartModel();
                item.sach = sach;
                item.Quantity = quantity;
                var list = new List<CartModel>();
                list.Add(item);

                //gán vào session
                Session[CartSession] = list;
            }

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

        //Thông tin khách hàng
        [HttpGet]
        [ChildActionOnly]
        public PartialViewResult UserInfo()
        {
            //lấy dữ liệu từ session
            var model = Session["User"];

            if (ModelState.IsValid)
            {
                //tìm tên tài khoản
                var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == model);

                //trả về dữ liệu tương ứng
                return PartialView(result);
            }

            return PartialView();
        }

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
            else
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
                return View(list);
            }
        }
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [HttpPost]
        public ActionResult Payment(int MaKH, FormCollection f)
        {
            var PMethod = int.Parse(f["PaymentMethod"]);
            var maVoucher = int.Parse(f["MaVoucher"] ?? "0");
            
            var order = new DonDatHang();
            order.NgayDat = DateTime.Now;
            order.NgayGiao = DateTime.Now.AddDays(3);
            order.TinhTrang = true; //đã nhận hàng
            order.MaKH = MaKH;
            order.GhiChu = f["GhiChu"];
            
            try
            {
                if (PMethod == 1)
                {
                    //thêm dữ liệu vào đơn đặt hàng
                    order.ThanhToan = 1;
                    order.TrangThaiDonHang = 0; // Chờ xác nhận
                    order.MaVoucher = null;
                    order.GiamGia = 0;
                    
                    var cart = (List<CartModel>)Session[CartSession];
                    decimal? total = cart.Sum(x => x.Total);
                    
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
                                    total.GetValueOrDefault(0), 
                                    MaKH
                                );
                                
                                if (voucherResult.Success)
                                {
                                    order.MaVoucher = maVoucher;
                                    order.GiamGia = (decimal)voucherResult.GiamGia;
                                }
                            }
                        }
                        catch
                        {
                            // Nếu voucher lỗi, vẫn cho đặt hàng bình thường
                        }
                    }
                    
                    var result1 = new OrderProcess().Insert(order);
                    
                    // Lưu chi tiết đơn hàng
                    var result2 = new OderDetailProcess();
                    foreach (var item in cart)
                    {
                        var orderDetail = new ChiTietDDH();
                        orderDetail.MaSach = item.sach.MaSach;
                        orderDetail.MaDDH = result1;
                        orderDetail.SoLuong = item.Quantity;
                        orderDetail.DonGia = item.sach.GiaBan;
                        result2.Insert(orderDetail);
                    }
                    
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

                    Session[CartSession] = null;
                    return Redirect("/Cart/Success");
                }
                else
                {
                    order.ThanhToan = 0;
                    var result1 = new OrderProcess().Insert(order);
                    var cart = (List<CartModel>)Session[CartSession];
                    var result2 = new OderDetailProcess();
                    decimal? total = 0;
                    foreach (var item in cart)
                    {
                        var orderDetail = new ChiTietDDH();
                        orderDetail.MaSach = item.sach.MaSach;
                        orderDetail.MaDDH = result1;
                        orderDetail.SoLuong = item.Quantity;
                        orderDetail.DonGia = item.sach.GiaBan;
                        result2.Insert(orderDetail);

                        total = cart.Sum(x => x.Total);
                    }
                    Session[CartSession] = null;
                    return Redirect(ThanhToanMoMo(result1.ToString(), 
                        total.ToString().Substring(0, total.ToString().Length - 5)));
                    

                }
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

            return new EmptyResult();

        }

        protected string ThanhToanMoMo(string maDonHang,string tongCong)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOHDRK20200430";
            string accessKey = "68tVdaHzCcvtfzwH";
            string serectkey = "8AWejATXBF96XL3CqeICtqiiKwheEUAv";
            string orderInfo = "OrderBook";
            string returnUrl = Url.Action("Success", "Cart", null, protocol: Request.Url.Scheme);
            string notifyurl = Url.Action("Index", "Home", null, protocol: Request.Url.Scheme);

            string amount = tongCong;
            string orderid = maDonHang;
            string requestId = maDonHang;
            string extraData = "";

            string rawHash = "partnerCode=" +
                             partnerCode + "&accessKey=" +
                             accessKey + "&requestId=" +
                             requestId + "&amount=" +
                             amount + "&orderId=" +
                             orderid + "&orderInfo=" +
                             orderInfo + "&returnUrl=" +
                             returnUrl + "&notifyUrl=" +
                             notifyurl + "&extraData=" +
                             extraData;

            log.Debug("rawHash = " + rawHash);
            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            log.Debug("Signature = " + signature);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };
            log.Debug("Json request to MoMo: " + message.ToString());
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);
            log.Debug("Return from MoMo: " + jmessage.ToString());
            
            return jmessage.GetValue("payUrl").ToString();
            
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

        public JsonResult loadOrder()
        {
            //if (id!=null)
            //{
            db.Configuration.ProxyCreationEnabled = false;
            var donDatHang = db.DonDatHangs.ToList();
            
            return Json(new {data= donDatHang }
                , JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    List<DonDatHang> donDatHang = db.DonDatHangs.Where(p => p.MaKH == UserController.khachhangstatic.MaKH).ToList();
            //    return Json(donDatHang, JsonRequestBehavior.AllowGet);
            //}
        }
    }
}