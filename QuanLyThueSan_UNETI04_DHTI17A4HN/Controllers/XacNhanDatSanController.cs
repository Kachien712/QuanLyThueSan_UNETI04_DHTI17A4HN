// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 1 - danh sách đơn chờ xác nhận và thao tác xác nhận/từ chối.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

public class XacNhanDatSanController(ApplicationDbContext db, XacNhanDatSanService service) : Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!CoQuyen()) return Forbid();
        var donCho = await db.DatSans.AsNoTracking()
            .Include(x => x.SanTheThao).Include(x => x.KhachHang)
            .Where(x => x.TrangThai == "Chờ xác nhận")
            .OrderBy(x => x.NgaySuDung).ThenBy(x => x.GioBatDau)
            .ToListAsync(ct);
        return View(donCho);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(int id, CancellationToken ct)
    {
        if (!CoQuyen()) return Forbid();
        var result = await service.XacNhanAsync(id, ct);
        TempData[result.ThanhCong ? "Success" : "Error"] = result.ThongBao;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(int id, string? lyDo, CancellationToken ct)
    {
        if (!CoQuyen()) return Forbid();
        var result = await service.TuChoiAsync(id, lyDo, ct);
        TempData[result.ThanhCong ? "Success" : "Error"] = result.ThongBao;
        return RedirectToAction(nameof(Index));
    }

    private bool CoQuyen()
    {
        var vaiTro = HttpContext.Session.GetString("VaiTro");
        return vaiTro is "Admin" or "NhanVien";
    }
}
