// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - ViewModel hiển thị chi tiết đơn đặt sân, tính tiền tạm tính và kiểm tra quyền hủy

using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class DatSanDetailsViewModel
    {
        public DatSan DonDatSan { get; set; } = null!;

        // Đơn giá áp dụng theo bảng giá (VNĐ/giờ)
        public decimal? DonGiaGio { get; set; }

        // Thời lượng sử dụng (giờ)
        public double ThoiLuongGio { get; set; }

        // Tiền sân tạm tính (hoặc thực tế theo hóa đơn thanh toán)
        public decimal TienSan { get; set; }

        // Tiền dịch vụ đã sử dụng (nếu có)
        public decimal TienDichVu { get; set; }

        // Tổng tiền dự kiến hoặc đã thanh toán
        public decimal TongTien { get; set; }

        // Quyền được hủy đơn (Khách hàng chỉ được hủy khi trạng thái "Chờ xác nhận" hoặc trước giờ quy định)
        public bool ChoPhepKhachHuy { get; set; }

        // Lý do không cho phép hủy nếu vi phạm quy định
        public string? LyDoKhongChoHuy { get; set; }
    }
}
