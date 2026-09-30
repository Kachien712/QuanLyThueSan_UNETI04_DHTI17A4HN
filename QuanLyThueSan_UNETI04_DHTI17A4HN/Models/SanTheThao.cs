// Họ và tên: [Họ tên SV2]
// Mã sinh viên: [MSSV SV2]
// Nội dung thực hiện: Module 2 - Entity Sân thể thao
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class SanTheThao
    {
        [Key]
        public int MaSan { get; set; }

        [Required(ErrorMessage = "Tên sân không được để trống")]
        [StringLength(100)]
        public string TenSan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại sân")]
        public int MaLoaiSan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khu vực")]
        public int MaKhuVuc { get; set; }

        [Range(1, 1000, ErrorMessage = "Sức chứa phải lớn hơn 0")]
        public int SucChua { get; set; }

        [StringLength(500)]
        public string? MoTa { get; set; }

        [Required]
        [StringLength(50)]
        public string TinhTrang { get; set; } = "Đang hoạt động"; // Đang hoạt động, Tạm ngừng, Đang bảo trì, Ngừng sử dụng

        [ForeignKey("MaLoaiSan")]
        public virtual LoaiSan? LoaiSan { get; set; }

        [ForeignKey("MaKhuVuc")]
        public virtual KhuVuc? KhuVuc { get; set; }

        public virtual ICollection<BangGia> BangGias { get; set; } = new List<BangGia>();
        public virtual ICollection<DatSan> DatSans { get; set; } = new List<DatSan>();
    }
}
