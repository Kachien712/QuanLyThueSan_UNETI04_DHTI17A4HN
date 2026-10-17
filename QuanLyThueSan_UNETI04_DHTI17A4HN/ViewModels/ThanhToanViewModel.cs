// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - ViewModel danh sách đơn đặt sân đang chờ thanh toán (có tìm kiếm và phân trang)
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    // Một dòng trong danh sách "Chờ thanh toán": chỉ lấy các cột cần hiển thị (tên thay cho mã khóa ngoại)
    public class DonChoThanhToanViewModel
    {
        public int MaDatSan { get; set; }
        public string TenKhachHang { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string TenSan { get; set; } = string.Empty;
        public DateTime NgaySuDung { get; set; }
        public TimeSpan GioBatDau { get; set; }
        public TimeSpan GioKetThuc { get; set; }
    }

    // Dữ liệu cả trang danh sách
    public class ChoThanhToanIndexViewModel
    {
        public PaginatedList<DonChoThanhToanViewModel>? DanhSachDon { get; set; }

        // Từ khóa tìm kiếm: mã đơn, tên khách, số điện thoại hoặc tên sân
        public string? TuKhoa { get; set; }
    }
}
