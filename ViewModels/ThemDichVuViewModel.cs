/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: ViewModel nhận yêu cầu thêm dịch vụ; đơn giá được lấy từ CSDL, không nhận từ biểu mẫu.
 */
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class ThemDichVuViewModel
{
    [Required]
    public int MaDatSan { get; set; }

    [Required(ErrorMessage = "Hãy chọn dịch vụ.")]
    [Display(Name = "Dịch vụ")]
    public int? MaDichVu { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    [Display(Name = "Số lượng")]
    public int SoLuong { get; set; } = 1;

    public IReadOnlyList<SelectListItem> DichVuOptions { get; set; } = [];
}
