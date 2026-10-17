// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - ViewModel quản lý danh sách đơn đặt sân (Tìm kiếm, Lọc, Sắp xếp, Phân trang LINQ, Thống kê)

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class DatSanIndexViewModel
    {
        // 1. Dữ liệu danh sách đơn đặt sân đã phân trang bằng PaginatedList LINQ
        public PaginatedList<DatSan> DanhSachDatSan { get; set; } = null!;

        // 2. Dữ liệu phục vụ TÌM KIẾM (Search)
        public string? TuKhoa { get; set; } // Tìm theo tên khách hàng, số điện thoại, tên sân

        // 3. Dữ liệu phục vụ BỘ LỌC (Filter)
        public DateTime? NgayFilter { get; set; } // Lọc theo ngày sử dụng
        public string? TrangThaiFilter { get; set; } // Lọc theo trạng thái đơn
        public int? MaLoaiSanFilter { get; set; } // Lọc theo loại sân
        public int? MaKhuVucFilter { get; set; } // Lọc theo khu vực
        public int? MaSanFilter { get; set; } // Lọc theo sân cụ thể

        // 4. Dữ liệu phục vụ SẮP XẾP (Sort)
        public string? SortOrder { get; set; } // date_asc, date_desc, created_desc, name_asc

        // 5. Danh sách DropDown phục vụ hiển thị bộ lọc trên giao diện
        public SelectList? DanhSachTrangThai { get; set; }
        public SelectList? DanhSachLoaiSan { get; set; }
        public SelectList? DanhSachKhuVuc { get; set; }
        public SelectList? DanhSachSan { get; set; }

        // 6. Thống kê nhanh số lượng đơn đặt sân theo các trạng thái
        public int TongSoDon { get; set; }
        public int SoDonChoXacNhan { get; set; }
        public int SoDonDaXacNhan { get; set; }
        public int SoDonDangSuDung { get; set; }
        public int SoDonChoThanhToan { get; set; }
        public int SoDonHoanThanh { get; set; }
        public int SoDonDaHuy { get; set; }
    }
}
