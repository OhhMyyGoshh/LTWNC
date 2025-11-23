using WebBanSach.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebBanSach.Models.Process;

namespace WebBanSach.Models.Process
{
    public class UserProcess
    {
        //Tầng xử lý dữ liệu khách hàng

        BSDBContext db = null;

        /// <summary>
        /// Contructor
        /// </summary>
        public UserProcess()
        {
            db = new BSDBContext();
        }

        /// <summary>
        /// hàm lấy mã khách hàng
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>KhachHang</returns>
        public KhachHang GetIdUser(int id)
        {
            return db.KhachHangs.Find(id);
        }

        /// <summary>
        /// Hàm thêm khách hàng mới
        /// </summary>
        /// <param name="entity">KhachHang</param>
        /// <returns>int</returns>
        public int InsertUser(KhachHang entity)
        {
            db.KhachHangs.Add(entity);
            db.SaveChanges();
            return entity.MaKH;
        }

        /// <summary>
        /// hàm đăng nhập của khách hàng
        /// Hỗ trợ đăng nhập bằng: Tài khoản (username), Email, hoặc Số điện thoại
        /// </summary>
        /// <param name="usernameOrEmailOrPhone">string - có thể là username, email hoặc số điện thoại</param>
        /// <param name="password">string</param>
        /// <returns>int: 1 = thành công, 0 = không tồn tại, -1 = sai mật khẩu</returns>
        public int Login(string usernameOrEmailOrPhone, string password)
        {
            // Loại bỏ khoảng trắng ở đầu và cuối
            if (string.IsNullOrWhiteSpace(usernameOrEmailOrPhone) || string.IsNullOrWhiteSpace(password))
            {
                return 0;
            }
            
            var input = usernameOrEmailOrPhone.Trim();
            var inputLower = input.ToLower(); // Chuyển sang chữ thường để so sánh
            
            // Tìm user theo: Tài khoản, Email, hoặc Số điện thoại
            // So sánh không phân biệt chữ hoa/thường và loại bỏ khoảng trắng
            var result = db.KhachHangs.FirstOrDefault(x => 
                (x.TaiKhoan != null && x.TaiKhoan.Trim().ToLower() == inputLower) || 
                (x.Email != null && x.Email.Trim().ToLower() == inputLower) || 
                (x.DienThoai != null && x.DienThoai.Trim() == input.Trim()));
            
            if (result == null)
            {
                return 0; // Không tìm thấy
            }
            else
            {
                if (result.MatKhau != null && result.MatKhau.Trim() == password.Trim())
                    return 1; // Thành công
                else
                    return -1; // Sai mật khẩu
            }
        }

        /// <summary>
        /// hàm kiểm tra đã tồn tại tài khoản trong db
        /// </summary>
        /// <param name="username">string</param>
        /// <param name="password">string</param>
        /// <returns>int</returns>
        public int CheckUsername(string username,string password)
        {
            var result = db.KhachHangs.SingleOrDefault(x => x.TaiKhoan == username);
            if(result == null)
            {
                return 0;
            }
            else
            {
                if(result.MatKhau == password)
                {
                    return 1;
                }
                return -1;
            }
        }

        /// <summary>
        /// hàm lưu thông tin cập nhật khách hàng
        /// </summary>
        /// <param name="entity">KhachHang</param>
        /// <returns>int</returns>
        // Không dùng nữa, đã chuyển logic sang controller để xử lý file upload



    }
}