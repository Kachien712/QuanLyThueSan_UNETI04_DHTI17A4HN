// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Tìm kiếm, lọc, sắp xếp và phân trang hóa đơn
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class ThanhToanIndexViewModel
{
    public PaginatedList<ThanhToan> DanhSach { get; set; } = new([], 0, 1, 8);
    public string? TuKhoa { get; set; }
    public string? PhuongThuc { get; set; }
    public string? SortOrder { get; set; }
    public int PageSize { get; set; } = 8;
}
