// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 4 - Danh sách đơn đang dùng sân và thao tác dịch vụ phát sinh.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Services;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

[PhanQuyen("Admin", "NhanVien")]
public class SuDungDichVuController(ApplicationDbContext db, SuDungDichVuService service) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var donDangSuDung = await db.DatSans.AsNoTracking()
            .Include(x => x.KhachHang).Include(x => x.SanTheThao)
            .Where(x => x.TrangThai == "Đang sử dụng")
            .OrderByDescending(x => x.NgaySuDung).ThenBy(x => x.GioBatDau)
            .ToListAsync(ct);
        return View(donDangSuDung);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id, CancellationToken ct)
    {
        var don = await db.DatSans.AsNoTracking()
            .Include(x => x.KhachHang).Include(x => x.SanTheThao)
            .SingleOrDefaultAsync(x => x.MaDatSan == id && x.TrangThai == "Đang sử dụng", ct);
        if (don is null)
        {
            TempData["Error"] = "Đơn không tồn tại hoặc không còn ở trạng thái Đang sử dụng.";
            return RedirectToAction(nameof(Index));
        }

        var danhMuc = await db.DichVus.AsNoTracking()
            .Where(x => x.TrangThai && (!x.SoLuongTon.HasValue || x.SoLuongTon > 0))
            .OrderBy(x => x.TenDichVu).ToListAsync(ct);
        var phatSinh = await db.SuDungDichVus.AsNoTracking()
            .Include(x => x.DichVu)
            .Where(x => x.MaDatSan == id)
            .OrderByDescending(x => x.ThoiGianThem).ThenByDescending(x => x.MaSuDung)
            .ToListAsync(ct);
        return View(new SuDungDichVuChiTietViewModel
        {
            Don = don,
            DanhMuc = danhMuc,
            PhatSinh = phatSinh
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Them(int id, ThemSuDungDichVuViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Vui lòng chọn dịch vụ và nhập số lượng từ 1 đến 1000.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        var result = await service.ThemAsync(id, model.MaDichVu, model.SoLuong, ct);
        TempData[result.ThanhCong ? "Success" : "Error"] = result.ThongBao;
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id, int maSuDung, CancellationToken ct)
    {
        var result = await service.XoaAsync(id, maSuDung, ct);
        TempData[result.ThanhCong ? "Success" : "Error"] = result.ThongBao;
        return RedirectToAction(nameof(ChiTiet), new { id });
    }
}
