

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanSach.Models.Data;
using WebBanSach.Models.Process;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web.Hosting;
using WebBanSach.Areas.Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;

namespace WebBanSach.Controllers
{
    public class UserController : Controller
    {
        //Khởi tạo biến dữ liệu : db
        BSDBContext db = new BSDBContext();
        public static KhachHang khachhangstatic;

        // GET: /User/Login2FA
        [HttpGet]
        public ActionResult Login2FA()
        {
            var taiKhoan = TempData["2FA_TaiKhoan"] as string;
            if (string.IsNullOrEmpty(taiKhoan))
                return RedirectToAction("LoginPage");
            ViewBag.TaiKhoan = taiKhoan;
            return View();
        }

        // POST: /User/Login2FA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login2FA(string TaiKhoan, string OTPCode)
        {
            var kh = db.KhachHangs.FirstOrDefault(x => x.TaiKhoan == TaiKhoan);
            if (kh == null)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                return View();
            }
            if (string.IsNullOrEmpty(OTPCode) || kh.OTPCode != OTPCode || kh.OTPExpiry == null || kh.OTPExpiry < DateTime.Now)
            {
                ModelState.AddModelError("", "Mã xác thực không đúng hoặc đã hết hạn.");
                ViewBag.TaiKhoan = TaiKhoan;
                return View();
            }
            // Xác thực thành công, xóa OTP
            kh.OTPCode = null;
            kh.OTPExpiry = null;
            db.SaveChanges();
            // Đăng nhập hoàn tất
            Session["User"] = kh.TaiKhoan;
            khachhangstatic = kh;
            // Lấy lại trạng thái GhiNho từ TempData nếu có
            bool isPersistent = false;
            if (TempData["2FA_GhiNho"] != null)
                bool.TryParse(TempData["2FA_GhiNho"].ToString(), out isPersistent);
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            var identity = new System.Security.Claims.ClaimsIdentity(
                new[] {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, kh.TaiKhoan),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, kh.TaiKhoan)
                },
                DefaultAuthenticationTypes.ApplicationCookie
            );
            var props = new AuthenticationProperties { IsPersistent = isPersistent };
            if (isPersistent)
                props.ExpiresUtc = DateTime.UtcNow.AddDays(7);
            authManager.SignIn(props, identity);
            return RedirectToAction("Index", "Home");
        }

        // Quên mật khẩu - GET
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // Quên mật khẩu - POST
        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ViewBag.Message = "Vui lòng nhập email.";
                return View();
            }
            var user = db.KhachHangs.FirstOrDefault(x => x.Email == Email);
            if (user == null)
            {
                ViewBag.Message = "Email không tồn tại trong hệ thống.";
                return View();
            }
            // Sinh token ngẫu nhiên và hạn sử dụng 15 phút
            var token = Guid.NewGuid().ToString("N");
            user.ResetPasswordToken = token;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            db.SaveChanges();
            // Gửi email chứa link reset
            try
            {
                string subject = "Yêu cầu đặt lại mật khẩu - WebBanSach";
                string resetLink = Url.Action("ResetPassword", "User", new { email = Email, token = token }, protocol: Request.Url.Scheme);
                string body = $"Chào {user.TenKH},<br/><br/>Bạn vừa yêu cầu đặt lại mật khẩu cho tài khoản tại WebBanSach.<br/>Nếu đây là bạn, hãy nhấn vào link sau để đặt lại mật khẩu:<br/><a href='{resetLink}'>Đặt lại mật khẩu</a><br/><br/>Link này có hiệu lực trong 15 phút.";
                MailMessage mail = new MailMessage();
                mail.To.Add(Email);
                mail.From = new MailAddress("anhduy465@gmail.com");
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("anhduy465@gmail.com", "zduqaikpdrbjwjkk");
                smtp.EnableSsl = true;
                smtp.Send(mail);
                ViewBag.Message = "Hướng dẫn đặt lại mật khẩu đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư.";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Không gửi được email: " + ex.Message;
            }
            return View();
        }

        // GET: /User/ResetPassword
        [HttpGet]
        public ActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Link không hợp lệ.";
                return View();
            }
            var user = db.KhachHangs.FirstOrDefault(x => x.Email == email && x.ResetPasswordToken == token && x.ResetPasswordExpiry > DateTime.Now);
            if (user == null)
            {
                ViewBag.Message = "Link đã hết hạn hoặc không hợp lệ.";
                return View();
            }
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        // POST: /User/ResetPassword
        [HttpPost]
        public ActionResult ResetPassword(string email, string token, string newPassword)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(newPassword))
            {
                ViewBag.Message = "Dữ liệu không hợp lệ.";
                return View();
            }
            var user = db.KhachHangs.FirstOrDefault(x => x.Email == email && x.ResetPasswordToken == token && x.ResetPasswordExpiry > DateTime.Now);
            if (user == null)
            {
                ViewBag.Message = "Link đã hết hạn hoặc không hợp lệ.";
                return View();
            }
            user.MatKhau = newPassword;
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;
            db.SaveChanges();
            ViewBag.Message = "Đổi mật khẩu thành công. Bạn có thể đăng nhập với mật khẩu mới.";
            return View();
        }

        [HttpGet]
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //GET: /User/Register : đăng kí tài khoản thành viên
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        //POST: /User/Register : thực hiện lưu dữ liệu đăng ký tài khoản thành viên
        public ActionResult Register(KhachHang model)
        {
            string rePassword = Request["RePassword"];
            // Validate phía server
            bool valid = true;
            // Tài khoản: 4-50 ký tự, không ký tự đặc biệt
            if (string.IsNullOrWhiteSpace(model.TaiKhoan) || model.TaiKhoan.Length < 4 || model.TaiKhoan.Length > 50 || !System.Text.RegularExpressions.Regex.IsMatch(model.TaiKhoan, "^[a-zA-Z0-9_]+$"))
            {
                ModelState.AddModelError("TaiKhoan", "Tài khoản phải từ 4-50 ký tự, chỉ gồm chữ, số, dấu gạch dưới.");
                valid = false;
            }
            // Tên: 2-50 ký tự, không số, không ký tự đặc biệt
            if (string.IsNullOrWhiteSpace(model.TenKH) || model.TenKH.Length < 2 || model.TenKH.Length > 50 || System.Text.RegularExpressions.Regex.IsMatch(model.TenKH, @"\d") || !System.Text.RegularExpressions.Regex.IsMatch(model.TenKH, @"^[a-zA-ZÀ-ỹ\s]+$"))
            {
                ModelState.AddModelError("TenKH", "Tên chỉ gồm chữ cái, không chứa số/ký tự đặc biệt, 2-50 ký tự.");
                valid = false;
            }
            // Email
            if (string.IsNullOrWhiteSpace(model.Email) || !System.Text.RegularExpressions.Regex.IsMatch(model.Email, @"^[\w\.-]+@[\w\.-]+\.\w{2,}$"))
            {
                ModelState.AddModelError("Email", "Email không hợp lệ.");
                valid = false;
            }
            // Số điện thoại: đúng 10 số
            if (string.IsNullOrWhiteSpace(model.DienThoai) || !System.Text.RegularExpressions.Regex.IsMatch(model.DienThoai, @"^\d{10}$"))
            {
                ModelState.AddModelError("DienThoai", "Số điện thoại phải đúng 10 số.");
                valid = false;
            }
            // Mật khẩu mạnh: >=6 ký tự, có chữ hoa, số, ký tự đặc biệt
            if (string.IsNullOrWhiteSpace(model.MatKhau) || model.MatKhau.Length < 6
                || !System.Text.RegularExpressions.Regex.IsMatch(model.MatKhau, @"[A-Z]")
                || !System.Text.RegularExpressions.Regex.IsMatch(model.MatKhau, @"\d")
                || !System.Text.RegularExpressions.Regex.IsMatch(model.MatKhau, @"[^a-zA-Z0-9]"))
            {
                ModelState.AddModelError("MatKhau", "Mật khẩu phải từ 6 ký tự, có chữ hoa, số, ký tự đặc biệt.");
                valid = false;
            }
            // Nhập lại mật khẩu
            if (string.IsNullOrEmpty(rePassword) || model.MatKhau != rePassword)
            {
                ModelState.AddModelError("RePassword", "Mật khẩu nhập lại không khớp.");
                valid = false;
            }
            if (valid && ModelState.IsValid)
            {
                // Kiểm tra tài khoản đã tồn tại chưa
                var check = db.KhachHangs.FirstOrDefault(s => s.TaiKhoan == model.TaiKhoan || s.Email == model.Email);
                if (check == null)
                {
                    model.NgayTao = DateTime.Now;
                    model.TrangThai = false; // Chưa kích hoạt
                    // Sinh token kích hoạt
                    model.ResetPasswordToken = Guid.NewGuid().ToString("N");
                    model.ResetPasswordExpiry = DateTime.Now.AddDays(1); // Token kích hoạt có hiệu lực 1 ngày
                    db.KhachHangs.Add(model);
                    db.SaveChanges();
                    // Gửi email kích hoạt
                    string subject = "Kích hoạt tài khoản WebBanSach";
                    string link = Url.Action("Activate", "User", new { email = model.Email, token = model.ResetPasswordToken }, protocol: Request.Url.Scheme);
                    string body = $"Chào {model.TenKH},<br/>Cảm ơn bạn đã đăng ký tài khoản tại WebBanSach.<br/>Vui lòng nhấn vào link sau để kích hoạt tài khoản:<br/><a href='{link}'>Kích hoạt tài khoản</a><br/>Link có hiệu lực trong 24h.";
                    MailMessage mail = new MailMessage();
                    mail.To.Add(model.Email);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;
                    SendEmail(mail);
                    return RedirectToAction("KiemTraThongBaoKichHoat");
                }
                else
                {
                    ViewBag.error = "Tài khoản hoặc email đã tồn tại";
                    return View();
                }
            }
            return View();
        }

        // Kích hoạt tài khoản qua email
        [HttpGet]
        public ActionResult Activate(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                ViewBag.Message = "Link kích hoạt không hợp lệ.";
                return View();
            }
            var user = db.KhachHangs.FirstOrDefault(x => x.Email == email && x.ResetPasswordToken == token && x.ResetPasswordExpiry > DateTime.Now);
            if (user == null)
            {
                ViewBag.Message = "Link kích hoạt đã hết hạn hoặc không hợp lệ.";
                return View();
            }
            user.TrangThai = true;
            user.ResetPasswordToken = null;
            user.ResetPasswordExpiry = null;
            db.SaveChanges();
            ViewBag.Message = "Kích hoạt tài khoản thành công. Bạn có thể đăng nhập.";
            return View();
        }


        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("anhduy465@gmail.com", "zduqaikpdrbjwjkk");
            try
            {
                mail.From = new MailAddress("anhduy465@gmail.com");
                client.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public ActionResult ThongBaoKichHoat()
        {
            return View();
        }
        public ActionResult KiemTraThongBaoKichHoat()
        {
            return View();
        }

        //GET : /User/LoginPage : trang đăng nhập
        public ActionResult LoginPage()
        {
            return View();
        }

        //GET : /User/LoginOTP : trang đăng nhập bằng OTP
        [HttpGet]
        public ActionResult LoginOTP()
        {
            return View();
        }

        //POST : /User/SendOTP : gửi mã OTP
        [HttpPost]
        public JsonResult SendOTP(string emailOrPhone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailOrPhone))
                {
                    return Json(new { success = false, message = "Vui lòng nhập email hoặc số điện thoại." });
                }

                var input = emailOrPhone.Trim();
                var inputLower = input.ToLower();

                // Tìm user theo email hoặc số điện thoại
                var user = db.KhachHangs.FirstOrDefault(x =>
                    (x.Email != null && x.Email.Trim().ToLower() == inputLower) ||
                    (x.DienThoai != null && x.DienThoai.Trim() == input));

                if (user == null)
                {
                    return Json(new { success = false, message = "Email/Số điện thoại không tồn tại trong hệ thống." });
                }

                if (user.TrangThai == false)
                {
                    return Json(new { success = false, message = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản." });
                }

                // Sinh mã OTP 6 chữ số
                Random random = new Random();
                string otpCode = random.Next(100000, 999999).ToString();

                // Lưu OTP vào database (có hạn 5 phút)
                user.OTPCode = otpCode;
                user.OTPExpiry = DateTime.Now.AddMinutes(5);
                db.SaveChanges();

                // Lưu vào Session để backup
                Session["OTP_" + user.Email] = otpCode;
                Session["OTP_Expiry_" + user.Email] = DateTime.Now.AddMinutes(5);
                Session["OTP_UserEmail"] = user.Email;

                // Gửi email chứa mã OTP
                string subject = "Mã OTP đăng nhập - WebBanSach";
                string body = $@"
                    <div style='font-family: Arial, sans-serif; padding: 20px;'>
                        <h2 style='color: #0094ff;'>Mã OTP đăng nhập</h2>
                        <p>Xin chào <strong>{user.TenKH}</strong>,</p>
                        <p>Bạn đã yêu cầu đăng nhập bằng OTP. Mã OTP của bạn là:</p>
                        <div style='background-color: #f0f0f0; padding: 15px; text-align: center; margin: 20px 0; border-radius: 5px;'>
                            <h1 style='color: #0094ff; font-size: 32px; margin: 0; letter-spacing: 5px;'>{otpCode}</h1>
                        </div>
                        <p><strong>Lưu ý:</strong> Mã OTP này có hiệu lực trong <strong>5 phút</strong>.</p>
                        <p>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email này.</p>
                        <p style='color: #888; font-size: 12px; margin-top: 30px;'>Trân trọng,<br/>Đội ngũ WebBanSach</p>
                    </div>";

                MailMessage mail = new MailMessage();
                mail.To.Add(user.Email);
                mail.From = new MailAddress("anhduy465@gmail.com");
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SendEmail(mail);

                return Json(new { 
                    success = true, 
                    message = "Mã OTP đã được gửi tới email của bạn. Vui lòng kiểm tra hộp thư.",
                    email = user.Email // Trả về email để hiển thị (ẩn một phần)
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi gửi mã OTP: " + ex.Message });
            }
        }

        //POST : /User/VerifyOTPLogin : xác thực OTP và đăng nhập
        [HttpPost]
        public ActionResult VerifyOTPLogin(string emailOrPhone, string otpCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(emailOrPhone) || string.IsNullOrWhiteSpace(otpCode))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                    ViewBag.EmailOrPhone = emailOrPhone;
                    ViewBag.ShowStep2 = true;
                    return View("LoginOTP");
                }

                var input = emailOrPhone.Trim();
                var inputLower = input.ToLower();

                // Tìm user theo email hoặc số điện thoại
                var user = db.KhachHangs.FirstOrDefault(x =>
                    (x.Email != null && x.Email.Trim().ToLower() == inputLower) ||
                    (x.DienThoai != null && x.DienThoai.Trim() == input));

                if (user == null)
                {
                    ModelState.AddModelError("", "Email/Số điện thoại không tồn tại trong hệ thống.");
                    ViewBag.EmailOrPhone = emailOrPhone;
                    ViewBag.ShowStep2 = true;
                    return View("LoginOTP");
                }

                if (user.TrangThai == false)
                {
                    ModelState.AddModelError("", "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản.");
                    ViewBag.EmailOrPhone = emailOrPhone;
                    ViewBag.ShowStep2 = true;
                    return View("LoginOTP");
                }

                // Kiểm tra OTP từ database
                bool otpValid = false;
                if (user.OTPCode != null && user.OTPExpiry != null)
                {
                    if (user.OTPCode.Trim() == otpCode.Trim() && user.OTPExpiry > DateTime.Now)
                    {
                        otpValid = true;
                    }
                }

                // Nếu không hợp lệ từ DB, kiểm tra Session (backup)
                if (!otpValid)
                {
                    var sessionOTP = Session["OTP_" + user.Email] as string;
                    var sessionExpiry = Session["OTP_Expiry_" + user.Email] as DateTime?;
                    
                    if (sessionOTP != null && sessionExpiry != null)
                    {
                        if (sessionOTP.Trim() == otpCode.Trim() && sessionExpiry > DateTime.Now)
                        {
                            otpValid = true;
                        }
                    }
                }

                if (!otpValid)
                {
                    ModelState.AddModelError("", "Mã OTP không đúng hoặc đã hết hạn. Vui lòng thử lại.");
                    // Luôn giữ lại ở bước nhập OTP (step 2)
                    ViewBag.EmailOrPhone = emailOrPhone;
                    ViewBag.ShowStep2 = true;
                    ViewBag.SentEmail = user.Email; // Để hiển thị email đã gửi
                    return View("LoginOTP");
                }

                // OTP hợp lệ - đăng nhập thành công
                // Xóa OTP sau khi sử dụng
                user.OTPCode = null;
                user.OTPExpiry = null;
                db.SaveChanges();

                // Xóa session OTP
                Session["OTP_" + user.Email] = null;
                Session["OTP_Expiry_" + user.Email] = null;
                Session["OTP_UserEmail"] = null;

                // Đăng nhập
                Session["User"] = user.TaiKhoan;
                khachhangstatic = user;

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi xử lý: " + ex.Message);
                ViewBag.ShowStep2 = true; // Giữ ở step 2 nếu đã có email
                if (!string.IsNullOrEmpty(emailOrPhone))
                {
                    ViewBag.EmailOrPhone = emailOrPhone;
                }
                return View("LoginOTP");
            }
        }

        // Action helper để debug - kiểm tra email có tồn tại không (chỉ dùng để test)
        [HttpPost]
        public JsonResult CheckEmailExists(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Json(new { exists = false, message = "Email trống" });
            }

            var input = email.Trim().ToLower();
            var user = db.KhachHangs.FirstOrDefault(x => 
                x.Email != null && x.Email.Trim().ToLower() == input);

            if (user != null)
            {
                return Json(new { 
                    exists = true, 
                    message = "Email tồn tại",
                    emailInDb = user.Email,
                    taiKhoan = user.TaiKhoan,
                    trangThai = user.TrangThai
                });
            }
            else
            {
                // Tìm tất cả email để debug
                var allEmails = db.KhachHangs
                    .Where(x => x.Email != null)
                    .Select(x => new { x.Email, x.TaiKhoan })
                    .Take(10)
                    .ToList();
                
                return Json(new { 
                    exists = false, 
                    message = "Email không tồn tại",
                    inputEmail = input,
                    sampleEmails = allEmails
                });
            }
        }

        //POST : /User/LoginPage : thực hiện đăng nhập
        [HttpPost]
        public ActionResult LoginPage(LoginModel model)
        {
            //kiểm tra hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                // Sử dụng UserProcess.Login để hỗ trợ đăng nhập bằng username/email/số điện thoại
                var result = new UserProcess().Login(model.TaiKhoan, model.MatKhau);
                if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản/Email/Số điện thoại không tồn tại.");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Mật khẩu không chính xác.");
                }
                else if (result == 1)
                {
                    // Tìm lại user để lấy thông tin đầy đủ
                    var input = model.TaiKhoan.Trim();
                    var inputLower = input.ToLower();
                    var kh = db.KhachHangs.FirstOrDefault(x => 
                        (x.TaiKhoan != null && x.TaiKhoan.Trim().ToLower() == inputLower) || 
                        (x.Email != null && x.Email.Trim().ToLower() == inputLower) || 
                        (x.DienThoai != null && x.DienThoai.Trim() == input));
                    if (kh == null)
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin người dùng.");
                    }
                    else if (kh.TrangThai == false)
                    {
                        // false + còn ResetPasswordToken => tài khoản mới đăng ký, chưa kích hoạt
                        if (!string.IsNullOrEmpty(kh.ResetPasswordToken))
                        {
                            ModelState.AddModelError("",
                                "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản.");
                        }
                        else
                        {
                            // false + ResetPasswordToken == null => tài khoản đã từng kích hoạt nhưng đã bị khoá bởi Admin
                            ModelState.AddModelError("",
                                "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                        }
                    }

                    else if (kh.Enable2FA)
                    {
                        // Sinh mã OTP 6 số, hiệu lực 5 phút
                        var otp = new Random().Next(100000, 999999).ToString();
                        kh.OTPCode = otp;
                        kh.OTPExpiry = DateTime.Now.AddMinutes(5);
                        db.SaveChanges();
                        // Gửi OTP qua email
                        try
                        {
                            string subject = "Mã xác thực đăng nhập (2FA) - WebBanSach";
                            string body = $"Mã xác thực đăng nhập của bạn là: <b>{otp}</b> (hiệu lực 5 phút). Nếu không phải bạn thực hiện, hãy bỏ qua email này.";
                            MailMessage mail = new MailMessage();
                            mail.To.Add(kh.Email);
                            mail.From = new MailAddress("anhduy465@gmail.com");
                            mail.Subject = subject;
                            mail.Body = body;
                            mail.IsBodyHtml = true;
                            SmtpClient smtp = new SmtpClient();
                            smtp.Host = "smtp.gmail.com";
                            smtp.Port = 587;
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential("anhduy465@gmail.com", "zduqaikpdrbjwjkk");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Không gửi được mã xác thực: " + ex.Message);
                            return View();
                        }
                        // Lưu thông tin tạm vào TempData để chuyển sang bước nhập OTP
                        TempData["2FA_TaiKhoan"] = kh.TaiKhoan;
                        TempData["2FA_GhiNho"] = model.GhiNho;
                        return RedirectToAction("Login2FA");
                    }
                    else
                    {
                        Session["User"] = kh.TaiKhoan;
                        khachhangstatic = kh;
                        // Remember Me: set persistent OWIN cookie if checked
                        var ctx = Request.GetOwinContext();
                        var authManager = ctx.Authentication;
                        var identity = new System.Security.Claims.ClaimsIdentity(
                            new[] {
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, kh.TaiKhoan),
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, kh.TaiKhoan)
                            },
                            DefaultAuthenticationTypes.ApplicationCookie
                        );
                        var isPersistent = model.GhiNho;
                        var props = new AuthenticationProperties { IsPersistent = isPersistent };
                        if (isPersistent)
                            props.ExpiresUtc = DateTime.UtcNow.AddDays(7);
                        authManager.SignIn(props, identity);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View();
        }


        //GET : /User/Login : đăng nhập tài khoản (hiển thị menu tài khoản)
        [ChildActionOnly]
        public ActionResult Login()
        {
            string avatarPath = Url.Content("~/Content/Client/img/avatar-default.svg");
            if (Session["User"] != null)
            {
                string taiKhoan = Session["User"] as string;
                var user = db.KhachHangs.FirstOrDefault(x => x.TaiKhoan == taiKhoan);
                if (user != null && !string.IsNullOrEmpty(user.Avatar))
                {
                    avatarPath = Url.Content(user.Avatar);
                }
                ViewBag.AvatarPath = avatarPath;
                // When logged in, return null as model (not used in view)
                return PartialView(null);
            }
            ViewBag.AvatarPath = avatarPath;
            return PartialView(new WebBanSach.Areas.Admin.Models.LoginModel());
        }

        //POST : /User/Login : thực hiện đăng nhập
        [HttpPost]
        [ChildActionOnly]
        public ActionResult Login(LoginModel model)
        {
            // Kiểm tra hợp lệ dữ liệu
            if (ModelState.IsValid)
            {
                var result = new UserProcess().Login(model.TaiKhoan, model.MatKhau);
                if (result == 1)
                {
                    var input = model.TaiKhoan.Trim();
                    var inputLower = input.ToLower();
                    var kh = db.KhachHangs.FirstOrDefault(x => 
                        (x.TaiKhoan != null && x.TaiKhoan.Trim().ToLower() == inputLower) || 
                        (x.Email != null && x.Email.Trim().ToLower() == inputLower) || 
                        (x.DienThoai != null && x.DienThoai.Trim() == input));
                    if (kh != null && kh.TrangThai == true)
                    {
                        Session["User"] = kh.TaiKhoan;
                        khachhangstatic = kh;
                        // Remember Me: set persistent OWIN cookie if checked
                        var ctx = Request.GetOwinContext();
                        var authManager = ctx.Authentication;
                        var identity = new System.Security.Claims.ClaimsIdentity(
                            new[] {
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, kh.TaiKhoan),
                                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, kh.TaiKhoan)
                            },
                            DefaultAuthenticationTypes.ApplicationCookie
                        );
                        var isPersistent = model.GhiNho;
                        var props = new AuthenticationProperties { IsPersistent = isPersistent };
                        if (isPersistent)
                            props.ExpiresUtc = DateTime.UtcNow.AddDays(7);
                        authManager.SignIn(props, identity);
                        ViewBag.LoginSuccess = true;
                    }
                    else if (kh != null && kh.TrangThai == false)
                    {
                        if (!string.IsNullOrEmpty(kh.ResetPasswordToken))
                        {
                            ModelState.AddModelError("",
                                "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản.");
                        }
                        else
                        {
                            ModelState.AddModelError("",
                                "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên để được hỗ trợ.");
                        }
                    }

                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy thông tin người dùng.");
                    }
                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản/Email/Số điện thoại không tồn tại.");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Mật khẩu không chính xác.");
                }
            }
            return PartialView();
        }

        //GET : /User/Logout : đăng xuất tài khoản khách hàng
        [HttpGet]
        public ActionResult Logout()
        {
            // Xóa tất cả session
            Session.Clear();
            Session.Abandon();
            
            // Xóa cookie authentication nếu có
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                var sessionCookie = new HttpCookie("ASP.NET_SessionId")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };
                Response.Cookies.Add(sessionCookie);
            }
            
            // Xóa external login cookie (Google OAuth)
            if (Request.Cookies[".AspNet.ExternalCookie"] != null)
            {
                var externalCookie = new HttpCookie(".AspNet.ExternalCookie")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };
                Response.Cookies.Add(externalCookie);
            }
            
            // Xóa application cookie nếu có
            if (Request.Cookies[".AspNet.ApplicationCookie"] != null)
            {
                var appCookie = new HttpCookie(".AspNet.ApplicationCookie")
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };
                Response.Cookies.Add(appCookie);
            }
            
            // Xóa static user
            khachhangstatic = null;
            
            // Sign out từ OWIN authentication nếu có
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut();
            
            return RedirectToAction("Index", "Home");
        }

        //GET : /User/EditUser : cập nhật thông tin khách hàng
        [HttpGet]
        public ActionResult EditUser()
        {
            var username = Session["User"] as string;
            if (string.IsNullOrEmpty(username))
                return RedirectToAction("LoginPage");
            var kh = db.KhachHangs.FirstOrDefault(x => x.TaiKhoan == username);
            if (kh == null)
                return RedirectToAction("LoginPage");
            // Đảm bảo menu tài khoản luôn lấy avatar mới nhất
            if (kh != null)
            {
                if (string.IsNullOrEmpty(kh.Avatar))
                    ViewBag.AvatarPath = Url.Content("~/Content/Client/img/avatar-default.svg");
                else
                    ViewBag.AvatarPath = Url.Content(kh.Avatar);
            }
            return View(kh);
        }

        [HttpPost]
        public ActionResult EditUser(KhachHang model, HttpPostedFileBase AvatarFile)
        {
            var kh = db.KhachHangs.Find(model.MaKH);
            if (kh == null)
                return RedirectToAction("LoginPage");

            // Nếu bấm nút xóa avatar
            if (Request["DeleteAvatar"] == "1")
            {
                if (!string.IsNullOrEmpty(kh.Avatar))
                {
                    var serverPath = Server.MapPath(kh.Avatar);
                    if (System.IO.File.Exists(serverPath))
                    {
                        System.IO.File.Delete(serverPath);
                    }
                    kh.Avatar = null;
                }
                db.SaveChanges();
                // Cập nhật lại avatar cho menu tài khoản (gọi lại action Login để cập nhật PartialView)
                // Sau khi xóa, chuyển hướng về EditUser để tránh view bị giữ cache
                return RedirectToAction("EditUser");
            }

            if (ModelState.IsValid)
            {
                kh.TenKH = model.TenKH;
                kh.Email = model.Email;
                kh.DiaChi = model.DiaChi;
                kh.DienThoai = model.DienThoai;
                kh.NgaySinh = model.NgaySinh;
                kh.Enable2FA = model.Enable2FA; // cập nhật trạng thái 2FA
                // Xử lý upload ảnh đại diện
                if (AvatarFile != null && AvatarFile.ContentLength > 0)
                {
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
                    var ext = System.IO.Path.GetExtension(AvatarFile.FileName).ToLower();
                    if (Array.IndexOf(allowedExtensions, ext) < 0)
                    {
                        ModelState.AddModelError("", "Chỉ hỗ trợ các định dạng ảnh: jpg, jpeg, png, gif, bmp, webp.");
                        return View(kh);
                    }
                    var fileName = System.IO.Path.GetFileName(AvatarFile.FileName);
                    var path = "/Content/Client/img/avatar/" + kh.MaKH + "_" + fileName;
                    var serverPath = Server.MapPath(path);
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(serverPath));
                    AvatarFile.SaveAs(serverPath);
                    kh.Avatar = path;
                }
                db.SaveChanges();
                // Cập nhật lại session và static user để menu tài khoản luôn lấy avatar mới nhất
                Session["User"] = kh.TaiKhoan;
                WebBanSach.Controllers.UserController.khachhangstatic = kh;
                if (string.IsNullOrEmpty(kh.Avatar))
                    ViewBag.AvatarPath = Url.Content("~/Content/Client/img/avatar-default.svg");
                else
                    ViewBag.AvatarPath = Url.Content(kh.Avatar);
                ViewBag.Message = "Cập nhật thành công!";
                return RedirectToAction("EditUser");
            }
            // Nếu model không hợp lệ, vẫn đảm bảo avatar menu đúng
            if (string.IsNullOrEmpty(model.Avatar))
                ViewBag.AvatarPath = Url.Content("~/Content/Client/img/avatar-default.svg");
            else
                ViewBag.AvatarPath = Url.Content(model.Avatar);
            return View(model);
        }
    }
}