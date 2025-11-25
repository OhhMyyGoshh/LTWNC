using System;
using System.Linq;
using System.Web.Mvc;
using WebBanSach.Models.Data;

namespace WebBanSach.Controllers
{
    public class LuckyWheelController : Controller
    {
        private readonly BSDBContext db = new BSDBContext();

        // GET: /LuckyWheel
        public ActionResult Index()
        {
            // Bắt buộc đăng nhập
            if (UserController.khachhangstatic == null)
            {
                return RedirectToAction("LoginPage", "User");
            }

            var segments = db.VongQuayVouchers
                             .Where(x => x.HoatDong)
                             .OrderBy(x => x.Id)
                             .ToList();

            if (!segments.Any())
            {
                ViewBag.Message = "Vòng quay hiện chưa được cấu hình. Vui lòng quay lại sau.";
            }

            return View(segments);
        }

        // POST: /LuckyWheel/Spin
        [HttpPost]
        public JsonResult Spin()
        {
            if (UserController.khachhangstatic == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để quay." });
            }

            int maKH = UserController.khachhangstatic.MaKH;

            // Load danh sách ô đang hoạt động (cùng thứ tự với Index)
            var segments = db.VongQuayVouchers
                             .Where(x => x.HoatDong)
                             .OrderBy(x => x.Id)
                             .ToList();

            if (!segments.Any())
            {
                return Json(new { success = false, message = "Vòng quay chưa được cấu hình." });
            }

            // RANDOM THEO TỈ LỆ TRÚNG (TiLeTrung)
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            int totalWeight = segments.Sum(s => s.TiLeTrung > 0 ? s.TiLeTrung : 0);
            if (totalWeight <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Tỉ lệ trúng của các ô đang không hợp lệ."
                });
            }

            int roll = rnd.Next(1, totalWeight + 1);

            int cumulative = 0;
            int winIndex = -1;
            for (int i = 0; i < segments.Count; i++)
            {
                var seg = segments[i];
                if (seg.TiLeTrung <= 0) continue;

                cumulative += seg.TiLeTrung;
                if (roll <= cumulative)
                {
                    winIndex = i;
                    break;
                }
            }

            if (winIndex < 0) winIndex = segments.Count - 1;
            var win = segments[winIndex];

            // Lưu log lượt quay
            var luot = new LuotQuayVoucher
            {
                MaKH = maKH,
                VongQuayVoucherId = win.Id,
                NgayQuay = DateTime.Now
            };
            db.LuotQuayVouchers.Add(luot);

            // Nếu ô không gắn voucher => TRƯỢT
            if (!win.MaVoucher.HasValue)
            {
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    hasVoucher = false,
                    winIndex,
                    totalSegments = segments.Count,
                    message = "Rất tiếc, lần này bạn chưa trúng voucher. Thử lại lần sau nhé!"
                });
            }

            // Lấy voucher tương ứng
            var voucher = db.Vouchers.Find(win.MaVoucher.Value);
            if (voucher == null || voucher.TrangThai == false || voucher.SoLuong <= 0)
            {
                db.SaveChanges();
                return Json(new
                {
                    success = true,
                    hasVoucher = false,
                    winIndex,
                    totalSegments = segments.Count,
                    message = "Ô này đã hết voucher hoặc không còn hiệu lực. Thử quay lại lần nữa nhé!"
                });
            }

            // TRÚNG VOUCHER: trừ tồn, đưa vào kho voucher người dùng (VoucherSuDung, MaDDH = null)
            voucher.SoLuong--;

            var userVoucher = new VoucherSuDung
            {
                MaVoucher = voucher.MaVoucher,
                MaKH = maKH,
                MaDDH = null,          // chưa gắn vào đơn nào
                NgaySuDung = null      // chưa sử dụng
            };
            db.VoucherSuDungs.Add(userVoucher);

            db.SaveChanges();

            return Json(new
            {
                success = true,
                hasVoucher = true,
                winIndex,
                totalSegments = segments.Count,
                voucherId = voucher.MaVoucher,
                voucherCode = voucher.MaCode,
                voucherName = voucher.TenVoucher,
                message = "Chúc mừng! Bạn trúng voucher: " + voucher.TenVoucher +
                          " (Mã: " + voucher.MaCode + ")"
            });
        }
    }
}
