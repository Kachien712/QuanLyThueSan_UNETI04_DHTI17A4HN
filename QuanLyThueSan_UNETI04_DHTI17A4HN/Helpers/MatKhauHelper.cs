// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Mã hóa và kiểm tra mật khẩu (SHA256)

using System.Security.Cryptography;
using System.Text;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers
{
    public static class MatKhauHelper
    {
        // Băm mật khẩu thành chuỗi hex64 ký tự, không lưu/plain trong CSDL
        public static string MaHoa(string matKhau)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(matKhau));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        // So khớp mật khẩu người dùng nhập với mật khẩu đã lưu trong DB
        public static bool KiemTra(string matKhauDaLuu, string matKhauNhap)
        {
            if (string.IsNullOrWhiteSpace(matKhauDaLuu) || string.IsNullOrEmpty(matKhauNhap))
                return false;

            return matKhauDaLuu == MaHoa(matKhauNhap) || matKhauDaLuu == matKhauNhap;
        }
    }
}
