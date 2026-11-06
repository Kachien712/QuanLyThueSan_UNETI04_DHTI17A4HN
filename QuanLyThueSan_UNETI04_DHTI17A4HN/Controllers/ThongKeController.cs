// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Thống kê LINQ, thống nhất doanh thu chỉ lấy hóa đơn đã thanh toán
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

[PhanQuyen("Admin", "NhanVien")]
public class ThongKeController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var don = db.DatSans.AsNoTracking();
        var thu = db.ThanhToans.AsNoTracking().Where(t => t.TrangThaiThanhToan == "Đã thanh toán");
        var model = new ThongKeViewModel
        {
            TongDon = await don.CountAsync(),
            DonHuy = await don.CountAsync(d => d.TrangThai == "Đã hủy"),
            // Đếm tất cả lượt đặt kể cả đơn hủy, giống SQL đối chiếu của nhóm.
            LuotTheoSan = await don.GroupBy(d => new { d.MaSan, d.SanTheThao!.TenSan })
                .Select(g => new DongThongKe { Ma = g.Key.MaSan, Ten = g.Key.TenSan, SoLuot = g.Count() })
                .OrderByDescending(x => x.SoLuot).ThenBy(x => x.Ma).ToListAsync(),
            LuotTheoLoai = await don.GroupBy(d => new { d.SanTheThao!.MaLoaiSan, d.SanTheThao.LoaiSan!.TenLoaiSan })
                .Select(g => new DongThongKe { Ma = g.Key.MaLoaiSan, Ten = g.Key.TenLoaiSan, SoLuot = g.Count() })
                .OrderByDescending(x => x.SoLuot).ThenBy(x => x.Ma).ToListAsync(),
            LuotTheoKhuVuc = await don.GroupBy(d => new { d.SanTheThao!.MaKhuVuc, d.SanTheThao.KhuVuc!.TenKhuVuc })
                .Select(g => new DongThongKe { Ma = g.Key.MaKhuVuc, Ten = g.Key.TenKhuVuc, SoLuot = g.Count() })
                .OrderByDescending(x => x.SoLuot).ThenBy(x => x.Ma).ToListAsync(),
            // Model chưa có giờ trả sân thực tế: tổng giờ dùng lấy khung giờ của các đơn đã/đang dùng.
            GioTheoSan = await don.Where(d => d.TrangThai == "Đang sử dụng" || d.TrangThai == "Chờ thanh toán" || d.TrangThai == "Hoàn thành")
                .GroupBy(d => new { d.MaSan, d.SanTheThao!.TenSan })
                .Select(g => new DongThongKe { Ma = g.Key.MaSan, Ten = g.Key.TenSan, SoLuot = g.Count(),
                    GiaTri = g.Sum(d => (decimal)EF.Functions.DateDiffSecond(d.GioBatDau, d.GioKetThuc)) / 3600m })
                .OrderByDescending(x => x.GiaTri).ThenBy(x => x.Ma).ToListAsync(),
            ThuTheoSan = await thu.GroupBy(t => new { t.DatSan!.MaSan, t.DatSan.SanTheThao!.TenSan })
                .Select(g => new DongThongKe { Ma = g.Key.MaSan, Ten = g.Key.TenSan, GiaTri = g.Sum(t => t.TongTien) })
                .OrderByDescending(x => x.GiaTri).ThenBy(x => x.Ma).ToListAsync(),
            ThuTheoLoai = await thu.GroupBy(t => new { t.DatSan!.SanTheThao!.MaLoaiSan, t.DatSan.SanTheThao.LoaiSan!.TenLoaiSan })
                .Select(g => new DongThongKe { Ma = g.Key.MaLoaiSan, Ten = g.Key.TenLoaiSan, GiaTri = g.Sum(t => t.TongTien) })
                .OrderByDescending(x => x.GiaTri).ThenBy(x => x.Ma).ToListAsync(),
            KhachHang = await don.GroupBy(d => new { d.MaKhachHang, d.KhachHang!.HoTen })
                .Select(g => new DongThongKe { Ma = g.Key.MaKhachHang, Ten = g.Key.HoTen, SoLuot = g.Count() })
                .OrderByDescending(x => x.SoLuot).ThenBy(x => x.Ma).ToListAsync(),
            // Dịch vụ được phân loại bằng danh mục DichVu hiện có, không thêm schema mới.
            DichVu = await db.SuDungDichVus.AsNoTracking()
                .Where(s => s.DatSan!.ThanhToan != null && s.DatSan.ThanhToan.TrangThaiThanhToan == "Đã thanh toán")
                .GroupBy(s => new { s.MaDichVu, s.DichVu!.TenDichVu })
                .Select(g => new DongThongKe { Ma = g.Key.MaDichVu, Ten = g.Key.TenDichVu,
                    SoLuot = g.Sum(s => s.SoLuong), GiaTri = g.Sum(s => s.SoLuong * s.DonGia) })
                .OrderByDescending(x => x.GiaTri).ThenBy(x => x.Ma).ToListAsync()
        };
        var thang = await thu.Where(t => t.NgayThanhToan.HasValue)
            .GroupBy(t => new { t.NgayThanhToan!.Value.Year, t.NgayThanhToan.Value.Month })
            .Select(g => new { Nam = g.Key.Year, Thang = g.Key.Month, Tien = g.Sum(t => t.TongTien) })
            .OrderBy(x => x.Nam).ThenBy(x => x.Thang).ToListAsync();
        model.ThuTheoThang = thang.Select(x => new DongThongKe { Ten = $"{x.Thang:00}/{x.Nam}", GiaTri = x.Tien }).ToList();
        // Khung giờ = khoảng giờ bắt đầu–kết thúc của đơn, tránh tự suy diễn thành khung giá.
        var gio = await don.GroupBy(d => new { d.GioBatDau, d.GioKetThuc })
            .Select(g => new { BatDau = g.Key.GioBatDau, KetThuc = g.Key.GioKetThuc, SoLuot = g.Count() })
            .OrderByDescending(x => x.SoLuot).ThenBy(x => x.BatDau).ThenBy(x => x.KetThuc).ToListAsync();
        model.KhungGio = gio.Select(x => new DongThongKe { Ten = $"{x.BatDau:hh\\:mm} – {x.KetThuc:hh\\:mm}", SoLuot = x.SoLuot }).ToList();
        return View(model);
    }
}
