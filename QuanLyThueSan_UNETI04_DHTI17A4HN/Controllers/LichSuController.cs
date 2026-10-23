// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Lịch sử của khách đăng nhập, kiểm tra quyền sở hữu tại server
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

[PhanQuyen("KhachHang")]
public class LichSuController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> CuaToi(int? maKhachHang, int? id)
    {
        var maTaiKhoan = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
        var khach = await db.KhachHangs.AsNoTracking().FirstOrDefaultAsync(k => k.MaTaiKhoan == maTaiKhoan);
        if (khach == null) return Forbid();
        if ((maKhachHang.HasValue && maKhachHang != khach.MaKhachHang)
            || (id.HasValue && id != khach.MaKhachHang)) return Forbid();
        // Không dùng mã từ URL để truy vấn dữ liệu lịch sử.
        var don = await db.DatSans.AsNoTracking().Where(d => d.MaKhachHang == khach.MaKhachHang)
            .Include(d => d.KhachHang).Include(d => d.SanTheThao).Include(d => d.ThanhToan)
            .Include(d => d.SuDungDichVus).ThenInclude(s => s.DichVu).AsSplitQuery().ToListAsync();
        return View(don);
    }
}
