// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 3 - quản lý danh mục dịch vụ, validation và trạng thái.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

[PhanQuyen("Admin", "NhanVien")]
public class DichVuController(ApplicationDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? timKiem, bool? dangHoatDong, CancellationToken ct)
    {
        IQueryable<DichVu> query = db.DichVus.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(timKiem))
        {
            var term = timKiem.Trim();
            query = query.Where(x => x.TenDichVu.Contains(term));
        }
        if (dangHoatDong.HasValue)
            query = query.Where(x => x.TrangThai == dangHoatDong.Value);

        ViewBag.TimKiem = timKiem;
        ViewBag.DangHoatDong = dangHoatDong;
        return View(await query.OrderBy(x => x.TenDichVu).ToListAsync(ct));
    }

    [HttpGet]
    public IActionResult Create() => View("Form", new DichVuFormViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DichVuFormViewModel model, CancellationToken ct)
    {
        await KiemTraTenAsync(model, null, ct);
        if (!ModelState.IsValid) return View("Form", model);
        db.DichVus.Add(new DichVu
        {
            TenDichVu = model.TenDichVu.Trim(),
            DonViTinh = model.DonViTinh.Trim(),
            DonGia = model.DonGia,
            SoLuongTon = model.SoLuongTon,
            TrangThai = model.TrangThai
        });
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.TenDichVu), "Tên dịch vụ đã tồn tại hoặc dữ liệu không hợp lệ.");
            return View("Form", model);
        }
        TempData["Success"] = "Đã thêm dịch vụ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var item = await db.DichVus.AsNoTracking().SingleOrDefaultAsync(x => x.MaDichVu == id, ct);
        if (item is null) return NotFound();
        return View("Form", new DichVuFormViewModel
        {
            MaDichVu = item.MaDichVu,
            TenDichVu = item.TenDichVu,
            DonViTinh = item.DonViTinh,
            DonGia = item.DonGia,
            SoLuongTon = item.SoLuongTon,
            TrangThai = item.TrangThai
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DichVuFormViewModel model, CancellationToken ct)
    {
        if (id != model.MaDichVu) return BadRequest();
        await KiemTraTenAsync(model, id, ct);
        if (!ModelState.IsValid) return View("Form", model);
        var item = await db.DichVus.SingleOrDefaultAsync(x => x.MaDichVu == id, ct);
        if (item is null) return NotFound();
        item.TenDichVu = model.TenDichVu.Trim();
        item.DonViTinh = model.DonViTinh.Trim();
        item.DonGia = model.DonGia;
        item.SoLuongTon = model.SoLuongTon;
        item.TrangThai = model.TrangThai;
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.TenDichVu), "Tên dịch vụ đã tồn tại hoặc dữ liệu không hợp lệ.");
            return View("Form", model);
        }
        TempData["Success"] = "Đã cập nhật dịch vụ.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThai(int id, CancellationToken ct)
    {
        var item = await db.DichVus.SingleOrDefaultAsync(x => x.MaDichVu == id, ct);
        if (item is null) return NotFound();
        item.TrangThai = !item.TrangThai;
        await db.SaveChangesAsync(ct);
        TempData["Success"] = item.TrangThai ? "Đã kích hoạt dịch vụ." : "Đã ngừng dịch vụ.";
        return RedirectToAction(nameof(Index));
    }

    private async Task KiemTraTenAsync(DichVuFormViewModel model, int? excludedId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.TenDichVu))
        {
            ModelState.AddModelError(nameof(model.TenDichVu), "Tên dịch vụ không được để trống.");
            return;
        }
        if (string.IsNullOrWhiteSpace(model.DonViTinh))
            ModelState.AddModelError(nameof(model.DonViTinh), "Đơn vị tính không được để trống.");
        var name = model.TenDichVu.Trim();
        if (await db.DichVus.AnyAsync(x => x.MaDichVu != excludedId && x.TenDichVu == name, ct))
            ModelState.AddModelError(nameof(model.TenDichVu), "Tên dịch vụ đã tồn tại.");
    }
}
