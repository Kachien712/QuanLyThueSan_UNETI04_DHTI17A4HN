// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Entity Khu vực sân (Mục 5.5 file Word)
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class KhuVuc
    {
        [Key]
        public int MaKhuVuc { get; set; }

        [Required(ErrorMessage = "Tên khu vực không được để trống")]
        [StringLength(100)]
        public string TenKhuVuc { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ViTri { get; set; }

        [StringLength(255)]
        public string? MoTa { get; set; }

        public bool TrangThai { get; set; } = true; // true: Hoạt động, false: Ngừng sử dụng

        // 1 Khu vực có nhiều Sân thể thao
        public virtual ICollection<SanTheThao> SanTheThaos { get; set; } = new List<SanTheThao>();
    }
}
