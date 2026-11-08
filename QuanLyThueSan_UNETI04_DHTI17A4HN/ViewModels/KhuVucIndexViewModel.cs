// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - ViewModel danh sách khu vực có tìm kiếm, lọc và thống kê (Mục 5.5 file Word)

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class KhuVucIndexViewModel
    {
        public PaginatedList<KhuVuc> DanhSachKhuVuc { get; set; } = null!;
        public string? TuKhoa { get; set; }
        public bool? TrangThaiFilter { get; set; }

        public int TongSoKhuVuc { get; set; }
        public int SoKhuVucHoatDong { get; set; }
        public int SoKhuVucTamNgung { get; set; }

        public SelectList DanhSachTrangThai { get; set; } = null!;
    }
}
