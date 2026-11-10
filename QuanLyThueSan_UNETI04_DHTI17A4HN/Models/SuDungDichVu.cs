// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 4 - Entity ghi nhận dịch vụ sử dụng phát sinh.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class SuDungDichVu
    {
        [Key]
        public int MaSuDung { get; set; }

        [Required]
        public int MaDatSan { get; set; }

        [Required]
        public int MaDichVu { get; set; }

        [Range(1, 1000, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ThanhTien { get; set; }

        public DateTime ThoiGianThem { get; set; } = DateTime.Now;

        [ForeignKey("MaDatSan")]
        public virtual DatSan? DatSan { get; set; }

        [ForeignKey("MaDichVu")]
        public virtual DichVu? DichVu { get; set; }
    }
}
