// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - ViewModel danh sách loại sân có tìm kiếm, lọc và thống kê (Mục 5.4 file Word)

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class LoaiSanIndexViewModel
    {
        public PaginatedList<LoaiSan> DanhSachLoaiSan { get; set; } = null!;
        public string? TuKhoa { get; set; }
        public bool? TrangThaiFilter { get; set; }

        public int TongSoLoaiSan { get; set; }
        public int SoLoaiSanHoatDong { get; set; }
        public int SoLoaiSanTamNgung { get; set; }

        public SelectList DanhSachTrangThai { get; set; } = null!;
    }
}
