// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Controller thanh toán (hiện có: danh sách đơn "Chờ thanh toán")
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    // Chỉ Admin và Nhân viên được vào chức năng thanh toán (kiểm tra ở Controller, không chỉ ẩn nút trên View)
    [PhanQuyen("Admin", "NhanVien")]
    public class ThanhToanController : Controller
    {
        private const string TrangThaiChoThanhToan = "Chờ thanh toán";

        private readonly ApplicationDbContext _context;

        public ThanhToanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DANH SÁCH ĐƠN "CHỜ THANH TOÁN" (tìm kiếm + phân trang)
        public async Task<IActionResult> ChoThanhToan(string? tuKhoa, int pageIndex = 1, int pageSize = 8)
        {
            // Chặn giá trị trang bất hợp lệ từ URL
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 8;

            // Chỉ lấy đơn đang ở trạng thái "Chờ thanh toán"
            var query = _context.DatSans
                .AsNoTracking()
                .Where(d => d.TrangThai == TrangThaiChoThanhToan);

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim();
                // Nếu người dùng gõ số thì có thể là mã đơn
                var laMaDon = int.TryParse(keyword, out var maDon);

                query = query.Where(d =>
                    (laMaDon && d.MaDatSan == maDon)
                    || (d.KhachHang != null && (d.KhachHang.HoTen.Contains(keyword)
                                                || d.KhachHang.SoDienThoai.Contains(keyword)))
                    || (d.SanTheThao != null && d.SanTheThao.TenSan.Contains(keyword)));
            }

            // Chỉ chọn các cột cần hiển thị, đơn mới nhất lên đầu
            var danhSach = query
                .OrderByDescending(d => d.NgaySuDung)
                .ThenBy(d => d.GioBatDau)
                .Select(d => new DonChoThanhToanViewModel
                {
                    MaDatSan = d.MaDatSan,
                    TenKhachHang = d.KhachHang != null ? d.KhachHang.HoTen : "(không rõ)",
                    SoDienThoai = d.KhachHang != null ? d.KhachHang.SoDienThoai : string.Empty,
                    TenSan = d.SanTheThao != null ? d.SanTheThao.TenSan : "(không rõ)",
                    NgaySuDung = d.NgaySuDung,
                    GioBatDau = d.GioBatDau,
                    GioKetThuc = d.GioKetThuc
                });

            var viewModel = new ChoThanhToanIndexViewModel
            {
                DanhSachDon = await PaginatedList<DonChoThanhToanViewModel>.CreateAsync(danhSach, pageIndex, pageSize),
                TuKhoa = tuKhoa
            };

            return View(viewModel);
        }
    }
}
