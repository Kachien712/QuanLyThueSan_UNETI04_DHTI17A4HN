// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - ViewModel biểu mẫu thêm/sửa Khu vực (Mục 5.5 file Word)

using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class KhuVucFormViewModel
    {
        public int MaKhuVuc { get; set; }

        [Required(ErrorMessage = "Tên khu vực không được để trống")]
        [StringLength(100, ErrorMessage = "Tên khu vực tối đa 100 ký tự")]
        [Display(Name = "Tên khu vực")]
        public string TenKhuVuc { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Vị trí tối đa 100 ký tự")]
        [Display(Name = "Vị trí")]
        public string? ViTri { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;
    }
}
