using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using WebBanSach.Models.Data;

namespace WebBanSach.Controllers
{
    public class VoucherController : Controller
    {
        BSDBContext db = new BSDBContext();

        // GET: /Voucher/MyVouchers?page=1
        public ActionResult MyVouchers(int page = 1, int pageSize = 9)
        {
            if (UserController.khachhangstatic == null)
            {
                return RedirectToAction("LoginPage", "User");
            }

            int maKH = UserController.khachhangstatic.MaKH;
            var now = DateTime.Now;

            var query = db.VoucherSuDungs
                          .Include("Voucher")
                          .Where(x => x.MaKH == maKH)
                          .OrderByDescending(x => x.Id);

            // Lấy toàn bộ để tính thống kê + phân trang
            var all = query.ToList();

            int total = all.Count;
            int usableCount = 0;
            int usedCount = 0;
            int expiredCount = 0;

            // Đếm trạng thái THEO ĐÚNG LOGIC Ở VIEW
            foreach (var item in all)
            {
                var v = item.Voucher;
                bool isUsed = item.NgaySuDung != null || item.MaDDH > 0;

                bool isValidTime = false;
                bool isActive = false;
                bool isExpired = false;

                if (v != null)
                {
                    isValidTime = v.NgayBatDau <= now && v.NgayKetThuc >= now;
                    isActive = v.TrangThai ?? false;
                    isExpired = !isValidTime || !isActive;
                }
                else
                {
                    isExpired = true;
                }

                if (v == null)
                {
                    expiredCount++;
                }
                else if (isUsed)
                {
                    usedCount++;
                }
                else if (isExpired)
                {
                    expiredCount++;
                }
                else
                {
                    usableCount++;
                }
            }

            int totalPages = (int)Math.Ceiling(total / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pageData = all
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalCount = total;
            ViewBag.UsableCount = usableCount;
            ViewBag.UsedCount = usedCount;
            ViewBag.ExpiredCount = expiredCount;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View(pageData);
        }
    }
}
