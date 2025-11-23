using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(WebBanSach.Startup))]
namespace WebBanSach
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/LoginPage"),
                ExpireTimeSpan = System.TimeSpan.FromDays(7), // Cho phép cookie tồn tại 7 ngày nếu persistent
                SlidingExpiration = true // Tự động gia hạn nếu người dùng hoạt động
            });

            // Thêm cookie xác thực ngoài cho Google
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            var googleOptions = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "442848952992-dqbql6qau56qntpuvmll0re1v84e3iop.apps.googleusercontent.com",
                ClientSecret = "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",
                CallbackPath = new PathString("/Account/ExternalLoginCallback")
            };
            
            // Yêu cầu scope để lấy email và profile
            googleOptions.Scope.Add("email");
            googleOptions.Scope.Add("profile");
            
            app.UseGoogleAuthentication(googleOptions);
        }
    }
}
