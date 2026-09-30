// Họ và tên: [Họ tên SV1]
// Mã sinh viên: [MSSV SV1]
// Nội dung thực hiện: Module 1 - Entity Tài khoản người dùng

using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 - 50 ký tự")]
        public string TenDangNhap { get; set; } = string.Empty;
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255)]
        public string MatKhau { get; set; } = string.Empty;
        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string VaiTro { get; set; } = "KhachHang"; // Admin, NhanVien, KhachHang
        public bool TrangThai { get; set; } = true; // true: Hoạt động, false: Khóa
        // Navigation Property: 1 Tài khoản liên kết 0 hoặc 1 Khách hàng
        public virtual KhachHang? KhachHang { get; set; }
    }
}
