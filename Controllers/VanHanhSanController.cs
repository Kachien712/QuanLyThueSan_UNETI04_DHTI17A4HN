/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Controller Module 4 - xác nhận/từ chối đơn, nhận sân, trả sân và thêm dịch vụ sử dụng.
 */
using Microsoft.AspNetCore.Mvc;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models.Enums;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Services;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers;

[YeuCauVaiTro("Admin", "NhanVien", "NhanVienQuanLy")]
public class VanHanhSanController(IVanHanhSanService vanHanhSanService) : Controller
{
    private readonly IVanHanhSanService _vanHanhSanService = vanHanhSanService;

    [HttpGet]
    public async Task<IActionResult> Index(TrangThaiDatSan? trangThai, CancellationToken cancellationToken)
    {
        ViewBag.TrangThaiDangLoc = trangThai;
        var model = await _vanHanhSanService.GetDanhSachAsync(trangThai, cancellationToken);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> ChiTiet(int id, CancellationToken cancellationToken)
    {
        var model = await _vanHanhSanService.GetChiTietAsync(id, cancellationToken);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(int id, CancellationToken cancellationToken)
    {
        var result = await _vanHanhSanService.XacNhanAsync(id, cancellationToken);
        SetThongBao(result);
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(int id, string lyDo, CancellationToken cancellationToken)
    {
        var result = await _vanHanhSanService.TuChoiAsync(id, lyDo, cancellationToken);
        SetThongBao(result);
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NhanSan(int id, CancellationToken cancellationToken)
    {
        var result = await _vanHanhSanService.NhanSanAsync(id, cancellationToken);
        SetThongBao(result);
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> ThemDichVu(int id, CancellationToken cancellationToken)
    {
        var datSan = await _vanHanhSanService.GetChiTietAsync(id, cancellationToken);
        if (datSan is null)
        {
            return NotFound();
        }

        if (datSan.TrangThai != TrangThaiDatSan.DangSuDung)
        {
            TempData["Error"] = "Chỉ đơn Đang sử dụng mới được thêm dịch vụ.";
            return RedirectToAction(nameof(ChiTiet), new { id });
        }

        return View(new ThemDichVuViewModel
        {
            MaDatSan = id,
            DichVuOptions = await _vanHanhSanService.GetDichVuKhaDungAsync(cancellationToken)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ThemDichVu(ThemDichVuViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid || !model.MaDichVu.HasValue)
        {
            model.DichVuOptions = await _vanHanhSanService.GetDichVuKhaDungAsync(cancellationToken);
            return View(model);
        }

        var result = await _vanHanhSanService.ThemDichVuAsync(
            model.MaDatSan,
            model.MaDichVu.Value,
            model.SoLuong,
            cancellationToken);
        SetThongBao(result);
        return RedirectToAction(nameof(ChiTiet), new { id = model.MaDatSan });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TraSan(int id, CancellationToken cancellationToken)
    {
        var result = await _vanHanhSanService.TraSanAsync(id, cancellationToken);
        SetThongBao(result);
        return RedirectToAction(nameof(ChiTiet), new { id });
    }

    private void SetThongBao(OperationResult result)
    {
        TempData[result.ThanhCong ? "Success" : "Error"] = result.ThongBao;
    }
}
