// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 2 - danh sách và thao tác nhận sân.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

public class NhanSanController(ApplicationDbContext db, NhanSanService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!CoQuyen()) return Forbid();
        var danhSach = await db.DatSans.AsNoTracking()
            .Include(x => x.SanTheThao).Include(x => x.KhachHang)
            .Where(x => x.TrangThai == "Đã xác nhận")
            .OrderBy(x => x.NgaySuDung).ThenBy(x => x.GioBatDau)
            .ToListAsync(ct);
        return View(danhSach);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Nhan(int id, CancellationToken ct)
    {
        if (!CoQuyen()) return Forbid();
        var result = await service.NhanSanAsync(id, ct);
        TempData[result.ThanhCong ? "Success" : "Error"] = result.ThongBao;
        return RedirectToAction(nameof(Index));
    }

    private bool CoQuyen() => HttpContext.Session.GetString("VaiTro") is "Admin" or "NhanVien";
}
