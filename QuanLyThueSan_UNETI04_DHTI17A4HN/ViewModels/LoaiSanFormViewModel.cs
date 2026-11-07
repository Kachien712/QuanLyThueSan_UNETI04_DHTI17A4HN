// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - ViewModel biểu mẫu thêm/sửa Loại sân (Mục 5.4 file Word)

using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class LoaiSanFormViewModel
    {
        public int MaLoaiSan { get; set; }

        [Required(ErrorMessage = "Tên loại sân không được để trống")]
        [StringLength(100, ErrorMessage = "Tên loại sân tối đa 100 ký tự")]
        [Display(Name = "Tên loại sân")]
        public string TenLoaiSan { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Range(1, 100, ErrorMessage = "Số người tối đa phải từ 1 đến 100 người")]
        [Display(Name = "Số người tối đa")]
        public int? SoNguoiToiDa { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;
    }
}
