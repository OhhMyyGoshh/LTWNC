
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace WebBanSach.Controllers
{
	public class AccountController : Controller
	{
		public ActionResult Test500()
		{
			throw new Exception("Test lỗi 500 - kiểm tra hiển thị lỗi chi tiết");
		}

		public class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null) { }
			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}
			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }
			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
					properties.Dictionary["XsrfId"] = UserId;
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

		public ActionResult Test()
		{
			return Content("AccountController is working.");
		}

		private IAuthenticationManager AuthenticationManager
		{
			get { return HttpContext.GetOwinContext().Authentication; }
		}

		public ActionResult ExternalLogin(string provider)
		{
			return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account"));
		}

		public async Task<ActionResult> ExternalLoginCallback()
		{
			try
			{
				// Kiểm tra error từ Google OAuth
				var error = Request.QueryString["error"];
				if (!string.IsNullOrEmpty(error))
				{
					// Xử lý trường hợp user từ chối hoặc có lỗi từ Google
					var errorDescription = Request.QueryString["error_description"] ?? "";
					TempData["ErrorMessage"] = $"Đăng nhập Google thất bại: {error}. {errorDescription}";
					return RedirectToAction("ExternalLoginFailure");
				}

				// Kiểm tra code từ Google OAuth
				var code = Request.QueryString["code"];
				if (string.IsNullOrEmpty(code))
				{
					TempData["ErrorMessage"] = "Không nhận được mã xác thực từ Google. Vui lòng thử lại.";
					return RedirectToAction("ExternalLoginFailure");
				}

				// Lấy thông tin đăng nhập từ Google
				var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
				if (loginInfo == null)
				{
					TempData["ErrorMessage"] = "Không lấy được thông tin đăng nhập từ Google. Vui lòng thử lại.";
					return RedirectToAction("ExternalLoginFailure");
				}

				var emailGoogle = loginInfo.Email;
				var nameGoogle = loginInfo.ExternalIdentity?.Name;
				
				if (string.IsNullOrEmpty(emailGoogle))
				{
					TempData["ErrorMessage"] = "Google không trả về email. Vui lòng kiểm tra cấu hình OAuth và đảm bảo đã cấp quyền truy cập email.";
					return RedirectToAction("ExternalLoginFailure");
				}

				// Xử lý database
				var dbGoogle = new WebBanSach.Models.Data.BSDBContext();
				WebBanSach.Models.Data.KhachHang userGoogle = null;
				try
				{
					userGoogle = dbGoogle.KhachHangs.FirstOrDefault(x => x.Email == emailGoogle);
				}
				catch (Exception dbEx)
				{
					TempData["ErrorMessage"] = $"Lỗi kết nối database: {dbEx.Message}";
					return RedirectToAction("ExternalLoginFailure");
				}

				// Tạo user mới nếu chưa tồn tại
				if (userGoogle == null)
				{
					userGoogle = new WebBanSach.Models.Data.KhachHang
					{
						TenKH = nameGoogle ?? emailGoogle,
						Email = emailGoogle,
						TaiKhoan = emailGoogle,
						MatKhau = Guid.NewGuid().ToString("N"),
						TrangThai = true,
						Avatar = "/Content/Client/img/avatar-default.svg",
						NgayTao = DateTime.Now
					};
					try
					{
						dbGoogle.KhachHangs.Add(userGoogle);
						dbGoogle.SaveChanges();
					}
					catch (Exception dbEx)
					{
						TempData["ErrorMessage"] = $"Lỗi khi tạo tài khoản mới: {dbEx.Message}";
						return RedirectToAction("ExternalLoginFailure");
					}
				}

				// Đăng nhập thành công
				Session["User"] = userGoogle.TaiKhoan;
				WebBanSach.Controllers.UserController.khachhangstatic = userGoogle;
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				// Log exception chi tiết hơn
				var errorMsg = $"Lỗi xử lý đăng nhập Google: {ex.Message}";
				if (ex.InnerException != null)
				{
					errorMsg += $" | Chi tiết: {ex.InnerException.Message}";
				}
				TempData["ErrorMessage"] = errorMsg;
				return RedirectToAction("ExternalLoginFailure");
			}
		}

		public ActionResult ExternalLoginFailure()
		{
			ViewBag.ErrorMessage = TempData["ErrorMessage"];
			return View();
		}
	}
}


