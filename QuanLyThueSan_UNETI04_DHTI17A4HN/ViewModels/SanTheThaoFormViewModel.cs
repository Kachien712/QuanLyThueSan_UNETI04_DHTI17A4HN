// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - ViewModel dùng chung cho form Thêm mới và Chỉnh sửa Sân thể thao

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class SanTheThaoFormViewModel
    {
        public int MaSan { get; set; }

        [Required(ErrorMessage = "Tên sân không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sân tối đa 100 ký tự")]
        [Display(Name = "Tên sân")]
        public string TenSan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại sân")]
        [Display(Name = "Loại sân")]
        public int MaLoaiSan { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khu vực")]
        [Display(Name = "Khu vực")]
        public int MaKhuVuc { get; set; }

        [Range(1, 1000, ErrorMessage = "Sức chứa phải lớn hơn 0 và không vượt quá 1000")]
        [Display(Name = "Sức chứa (người)")]
        public int SucChua { get; set; } = 10;

        [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tình trạng sân")]
        [Display(Name = "Tình trạng")]
        public string TinhTrang { get; set; } = "Đang hoạt động";

        // Danh sách dropdown (nạp từ DB trong Controller)
        public SelectList? DanhSachLoaiSan { get; set; }
        public SelectList? DanhSachKhuVuc { get; set; }

        // Danh sách cố định tình trạng sân (Mục 6.1 file Word)
        public static readonly List<string> DanhSachTinhTrangCoDinh = new()
        {
            "Đang hoạt động",
            "Tạm ngừng",
            "Đang bảo trì",
            "Ngừng sử dụng"
        };
    }
}
