// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 3 - ViewModel quản lý danh sách khách hàng, tìm kiếm, lọc và phân trang

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class KhachHangIndexViewModel
    {
        // 1. Danh sách khách hàng đã phân trang (sử dụng PaginatedList<T> của nhóm)
        public PaginatedList<KhachHang>? DanhSachKhachHang { get; set; }

        // 2. Dữ liệu phục vụ TÌM KIẾM (Search theo Họ tên, Số điện thoại, Email)
        public string? TuKhoa { get; set; }

        // 3. Dữ liệu phục vụ BỘ LỌC (Filter theo trạng thái: null = Tất cả, true = Đang hoạt động, false = Bị khóa)
        public bool? TrangThaiFilter { get; set; }
        public SelectList? DanhSachTrangThai { get; set; }

        // 4. Dữ liệu phục vụ SẮP XẾP (Sort)
        public string? SortOrder { get; set; }

        // 5. Thống kê nhanh số lượng
        public int TongSoKhachHang { get; set; } = 0;
        public int SoKhachHangHoatDong { get; set; } = 0;
        public int SoKhachHangBiKhoa { get; set; } = 0;
    }
}
