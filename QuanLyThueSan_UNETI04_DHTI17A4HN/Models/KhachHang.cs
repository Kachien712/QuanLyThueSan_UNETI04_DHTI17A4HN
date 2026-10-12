// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 3 - Entity Khách hàng
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class KhachHang
    {
        [Key]
        public int MaKhachHang { get; set; }

        public int? MaTaiKhoan { get; set; } // Khóa ngoại liên kết bảng Tài khoản (nếu khách có tài khoản)

        [Required(ErrorMessage = "Họ tên khách hàng không được để trống")]
        [StringLength(100)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? DiaChi { get; set; }

        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }

        public virtual ICollection<DatSan> DatSans { get; set; } = new List<DatSan>();
    }
}
