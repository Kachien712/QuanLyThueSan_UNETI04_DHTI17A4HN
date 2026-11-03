// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Dashboard quản trị, doanh thu theo ngày thanh toán
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

[PhanQuyen("Admin", "NhanVien")]
public class DashboardController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var bayGio = DateTime.Now;
        var ngay = bayGio.Date;
        var ngayMai = ngay.AddDays(1);
        var dauThang = new DateTime(ngay.Year, ngay.Month, 1);
        var thangSau = dauThang.AddMonths(1);
        var thu = db.ThanhToans.AsNoTracking().Where(t => t.TrangThaiThanhToan == "Đã thanh toán");
        var model = new DashboardViewModel
        {
            Ngay = ngay,
            TongSoSan = await db.SanTheThaos.CountAsync(),
            SanTheoTinhTrang = await db.SanTheThaos.GroupBy(s => s.TinhTrang)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() }).ToDictionaryAsync(x => x.Ten, x => x.SoLuong),
            DonTheoTrangThai = await db.DatSans.GroupBy(d => d.TrangThai)
                .Select(g => new { Ten = g.Key, SoLuong = g.Count() }).ToDictionaryAsync(x => x.Ten, x => x.SoLuong),
            SoLuotDungTrongNgay = await db.DatSans.CountAsync(d => d.NgaySuDung >= ngay && d.NgaySuDung < ngayMai
                && (d.TrangThai == "Đang sử dụng" || d.TrangThai == "Chờ thanh toán" || d.TrangThai == "Hoàn thành")),
            SoDonDaHuy = await db.DatSans.CountAsync(d => d.TrangThai == "Đã hủy"),
            DoanhThuNgay = await thu.Where(t => t.NgayThanhToan >= ngay && t.NgayThanhToan < ngayMai).SumAsync(t => t.TongTien),
            DoanhThuThang = await thu.Where(t => t.NgayThanhToan >= dauThang && t.NgayThanhToan < thangSau).SumAsync(t => t.TongTien),
            LichSapToi = await db.DatSans.AsNoTracking().Include(d => d.SanTheThao).Include(d => d.KhachHang)
                .Where(d => d.NgaySuDung >= ngay && d.NgaySuDung < ngayMai && d.GioBatDau >= bayGio.TimeOfDay
                    && d.TrangThai == "Đã xác nhận").OrderBy(d => d.GioBatDau).ThenBy(d => d.MaDatSan).ToListAsync()
        };
        return View(model);
    }
}
