// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Entity Hóa đơn thanh toán
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class ThanhToan
    {
        [Key]
        public int MaThanhToan { get; set; }

        [Required]
        public int MaDatSan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienSan { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TienDichVu { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [Required]
        [StringLength(50)]
        public string PhuongThucThanhToan { get; set; } = "Tiền mặt"; // Tiền mặt, Chuyển khoản

        public DateTime? NgayThanhToan { get; set; }

        [Required]
        [StringLength(50)]
        public string TrangThaiThanhToan { get; set; } = "Chưa thanh toán"; // Chưa thanh toán, Đã thanh toán

        [StringLength(500)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaDatSan")]
        public virtual DatSan? DatSan { get; set; }
    }
}
