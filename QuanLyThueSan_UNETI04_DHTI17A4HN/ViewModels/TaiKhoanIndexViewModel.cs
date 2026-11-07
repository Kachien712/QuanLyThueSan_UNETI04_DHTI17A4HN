// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - ViewModel danh sách tài khoản cho Admin (Mục 5.1 - 5.3 file Word)

using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class TaiKhoanIndexViewModel
    {
        public PaginatedList<TaiKhoan> DanhSachTaiKhoan { get; set; } = null!;
        public string? TuKhoa { get; set; }
        public string? VaiTroFilter { get; set; }
        public bool? TrangThaiFilter { get; set; }

        public int TongSoTaiKhoan { get; set; }
        public int SoTaiKhoanHoatDong { get; set; }
        public int SoTaiKhoanBiKhoa { get; set; }
        public int SoAdmin { get; set; }
        public int SoNhanVien { get; set; }
        public int SoKhachHang { get; set; }

        public SelectList DanhSachVaiTro { get; set; } = null!;
        public SelectList DanhSachTrangThai { get; set; } = null!;
    }
}
