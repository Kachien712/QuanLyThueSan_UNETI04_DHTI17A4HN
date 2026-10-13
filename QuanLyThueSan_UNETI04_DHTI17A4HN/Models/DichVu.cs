// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 - Entity Danh mục dịch vụ
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class DichVu
    {
        [Key]
        public int MaDichVu { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        [StringLength(100)]
        public string TenDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        [StringLength(50)]
        public string DonViTinh { get; set; } = string.Empty; // Chai, Lon, Giờ, Bộ...

        [Range(0, 100000000, ErrorMessage = "Đơn giá không hợp lệ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
        public int? SoLuongTon { get; set; }

        public bool TrangThai { get; set; } = true;

        public virtual ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
    }
}
