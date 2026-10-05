// Họ và tên: [Họ tên SV1]
// Mã sinh viên: [MSSV SV1]
// Nội dung thực hiện: Module 1 - Entity Khu vực sân
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

        public bool TrangThai { get; set; } = true;

        // 1 Khu vực có nhiều Sân thể thao
        public virtual ICollection<SanTheThao> SanTheThaos { get; set; } = new List<SanTheThao>();
    }
}
