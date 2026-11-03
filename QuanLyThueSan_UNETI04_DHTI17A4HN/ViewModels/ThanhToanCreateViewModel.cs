// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Dữ liệu xác nhận thu tiền, không nhận số tiền từ client
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class ThanhToanCreateViewModel
{
    [Range(1, int.MaxValue)]
    public int MaDatSan { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán.")]
    [RegularExpression("^(Tiền mặt|Chuyển khoản)$", ErrorMessage = "Phương thức thanh toán không hợp lệ.")]
    public string PhuongThucThanhToan { get; set; } = "Tiền mặt";

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}
