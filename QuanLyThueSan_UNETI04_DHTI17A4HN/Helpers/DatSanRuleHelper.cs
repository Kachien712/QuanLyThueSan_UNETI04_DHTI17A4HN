// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - Helper kiểm tra quy tắc nghiệp vụ Đặt sân, Trùng lịch & Hủy đặt sân

using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers
{
    public static class DatSanRuleHelper
    {
        /// <summary>
        /// Thuật toán kiểm tra giao nhau giữa 2 khoảng thời gian:
        /// BatDauMoi < KetThucCu && KetThucMoi > BatDauCu
        /// (Bắt đầu mới nhỏ hơn kết thúc cũ VÀ kết thúc mới lớn hơn bắt đầu cũ)
        /// </summary>
        public static bool IsTimeOverlapping(TimeSpan batDauMoi, TimeSpan ketThucMoi, TimeSpan batDauCu, TimeSpan ketThucCu)
        {
            return batDauMoi < ketThucCu && ketThucMoi > batDauCu;
        }

        /// <summary>
        /// Kiểm tra đơn đặt sân có bị trùng lịch với danh sách đơn đã có hay không:
        /// - Cùng sân (MaSan)
        /// - Cùng ngày sử dụng (NgaySuDung.Date)
        /// - Trạng thái là "Chờ xác nhận", "Đã xác nhận", hoặc "Đang sử dụng" (Bỏ qua "Đã hủy")
        /// - Khác mã đơn hiện tại (khi cập nhật)
        /// - Khoảng thời gian giao nhau
        /// </summary>
        public static bool KiemTraTrungLichTrongDanhSach(
            IEnumerable<DatSan> danhSachDonHienCo,
            int maSan,
            DateTime ngaySuDung,
            TimeSpan gioBatDau,
            TimeSpan gioKetThuc,
            int? maDatSanHienTai = null)
        {
            return danhSachDonHienCo.Any(d =>
                d.MaSan == maSan
                && d.NgaySuDung.Date == ngaySuDung.Date
                && (d.TrangThai == "Chờ xác nhận" || d.TrangThai == "Đã xác nhận" || d.TrangThai == "Đang sử dụng")
                && (!maDatSanHienTai.HasValue || d.MaDatSan != maDatSanHienTai.Value)
                && IsTimeOverlapping(gioBatDau, gioKetThuc, d.GioBatDau, d.GioKetThuc));
        }

        /// <summary>
        /// Kiểm tra quy định hủy đặt sân (Commit 13):
        /// - Khách hàng chỉ được phép hủy đơn khi ở trạng thái "Chờ xác nhận"
        /// - Phải hủy trước thời điểm bắt đầu sử dụng tối thiểu 2 tiếng
        /// </summary>
        public static (bool ChoPhep, string? LyDo) KiemTraDieuKienHuy(
            string trangThai,
            DateTime ngaySuDung,
            TimeSpan gioBatDau,
            DateTime thoiDiemHienTai)
        {
            if (trangThai != "Chờ xác nhận")
            {
                return (false, $"Đơn đang ở trạng thái '{trangThai}', không thể hủy theo quy định.");
            }

            DateTime thoiDiemBatDau = ngaySuDung.Date.Add(gioBatDau);
            if (thoiDiemBatDau - thoiDiemHienTai < TimeSpan.FromHours(2))
            {
                return (false, "Đơn đặt sân chỉ được phép hủy trước giờ bắt đầu sử dụng tối thiểu 2 tiếng.");
            }

            return (true, null);
        }

        /// <summary>
        /// Kiểm tra tính hợp lệ của khung giờ đặt sân:
        /// - GioKetThuc > GioBatDau
        /// - Thời lượng tối thiểu 30 phút
        /// </summary>
        public static (bool HopLe, string? Loi) KiemTraKhungGio(TimeSpan gioBatDau, TimeSpan gioKetThuc)
        {
            if (gioKetThuc <= gioBatDau)
            {
                return (false, "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
            }

            if ((gioKetThuc - gioBatDau).TotalMinutes < 30)
            {
                return (false, "Thời gian thuê sân tối thiểu phải từ 30 phút trở lên.");
            }

            return (true, null);
        }
    }
}
