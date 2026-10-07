// Họ và tên: [Đào Duy Khánh]
// Mã sinh viên: [23103100220]
// Nội dung thực hiện: Module 2 - ViewModel phục vụ tìm kiếm, lọc, sắp xếp và phân trang danh sách sân

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class SanTheThaoIndexViewModel
    {
        // 1. Dữ liệu danh sách sân hiển thị trên trang hiện tại
        public IEnumerable<SanTheThao> DanhSachSan { get; set; } = new List<SanTheThao>();
        // 2. Dữ liệu phục vụ TÌM KIẾM (Search)
        public string? TuKhoa { get; set; } // Tìm theo tên sân, tên loại, tên khu vực
        // 3. Dữ liệu phục vụ BỘ LỌC (Filter)
        public int? MaLoaiSan { get; set; }      // Loại sân đang chọn
        public int? MaKhuVuc { get; set; }       // Khu vực đang chọn
        public string? TinhTrang { get; set; }    // Đang hoạt động, Tạm ngừng, Bảo trì...
        public int? SucChuaToiThieu { get; set; } // Lọc theo sức chứa
        // Danh sách để đổ vào thẻ <select> dropdown trên giao diện
        public SelectList? DanhSachLoaiSan { get; set; }
        public SelectList? DanhSachKhuVuc { get; set; }
        public SelectList? DanhSachTinhTrang { get; set; }
        // 4. Dữ liệu phục vụ SẮP XẾP (Sort)
        public string? SortOrder { get; set; } // Lưu tiêu chí sắp xếp hiện tại
        // 5. Dữ liệu phục vụ PHÂN TRANG (Pagination)
        public int TrangHienTai { get; set; } = 1;
        public int TongSoTrang { get; set; } = 1;
        public int TongSoSan { get; set; } = 0;
        public int KichThuocTrang { get; set; } = 6; // Mỗi trang hiển thị 6 sân
        // Hai thuộc tính kiểm tra xem có trang trước / trang sau không
        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongSoTrang;
    }
}
