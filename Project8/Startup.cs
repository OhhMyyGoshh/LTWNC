using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebBanSach.Startup))]

namespace WebBanSach
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Cấu hình xác thực OWIN, Google, Cookie, v.v. nếu cần
        }
    }
}
