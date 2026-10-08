// Họ và tên: [Họ tên SV1]
// Mã sinh viên: [MSSV SV1]
// Nội dung thực hiện: Module 1 - Entity Loại sân thể thao
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class LoaiSan
    {
        [Key]
        public int MaLoaiSan { get; set; }

        [Required(ErrorMessage = "Tên loại sân không được để trống")]
        [StringLength(100)]
        public string TenLoaiSan { get; set; } = string.Empty;

        [StringLength(255)]
        public string? MoTa { get; set; }

        [Range(1, 100, ErrorMessage = "Số người tối đa phải lớn hơn 0")]
        public int? SoNguoiToiDa { get; set; }

        public bool TrangThai { get; set; } = true; // true: Hoạt động, false: Ngừng sử dụng

        // 1 Loại sân có nhiều Sân thể thao
        public virtual ICollection<SanTheThao> SanTheThaos { get; set; } = new List<SanTheThao>();
    }
}