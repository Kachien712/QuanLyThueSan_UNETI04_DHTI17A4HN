// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 3 - Entity Đơn đặt sân
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class DatSan
    {
        [Key]
        public int MaDatSan { get; set; }

        [Required]
        public int MaKhachHang { get; set; }

        [Required]
        public int MaSan { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NgaySuDung { get; set; }

        [Required]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        public TimeSpan GioKetThuc { get; set; }

        public DateTime NgayDat { get; set; } = DateTime.Now;

        [Range(1, 1000, ErrorMessage = "Số người phải lớn hơn 0")]
        public int SoNguoi { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThai { get; set; } = "Chờ xác nhận"; 
        // Trạng thái: Chờ xác nhận, Đã xác nhận, Đang sử dụng, Chờ thanh toán, Hoàn thành, Đã hủy

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaKhachHang")]
        public virtual KhachHang? KhachHang { get; set; }

        [ForeignKey("MaSan")]
        public virtual SanTheThao? SanTheThao { get; set; }

        public virtual ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
        public virtual ThanhToan? ThanhToan { get; set; }
    }
}
