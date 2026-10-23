// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Controller thanh toán (hiện có: danh sách đơn "Chờ thanh toán")
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    // Chỉ Admin và Nhân viên được vào chức năng thanh toán (kiểm tra ở Controller, không chỉ ẩn nút trên View)
    [PhanQuyen("Admin", "NhanVien")]
    public class ThanhToanController : Controller
    {
        private const string TrangThaiChoThanhToan = "Chờ thanh toán";

        private readonly ApplicationDbContext _context;
        private readonly TinhTienService _tinhTien;

        public ThanhToanController(ApplicationDbContext context)
        {
            _context = context;
            _tinhTien = new TinhTienService(context);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int maDatSan, int? id)
        {
            if (maDatSan == 0) maDatSan = id ?? 0;
            return await HienHoaDonAsync(new ThanhToanCreateViewModel { MaDatSan = maDatSan });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ThanhToanCreateViewModel model)
        {
            if (!ModelState.IsValid) return await HienHoaDonAsync(model);

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Cập nhật có điều kiện giữ khóa đơn đến khi commit: hai yêu cầu không thể cùng thu tiền.
                var soDong = await _context.DatSans
                    .Where(d => d.MaDatSan == model.MaDatSan && d.TrangThai == TrangThaiChoThanhToan
                        && !_context.ThanhToans.Any(t => t.MaDatSan == d.MaDatSan))
                    .ExecuteUpdateAsync(s => s.SetProperty(d => d.TrangThai, "Hoàn thành"));
                if (soDong != 1)
                {
                    await transaction.RollbackAsync();
                    return BaoLoi("Đơn không tồn tại, không ở trạng thái Chờ thanh toán hoặc đã có hóa đơn/đã thanh toán.");
                }

                // Chỉ nhận mã đơn, phương thức, ghi chú. Mọi số tiền đều tính lại trên máy chủ.
                var tien = await _tinhTien.TinhTatCaAsync(model.MaDatSan);
                _context.ThanhToans.Add(new ThanhToan
                {
                    MaDatSan = model.MaDatSan, TienSan = tien.TienSan, TienDichVu = tien.TienDichVu,
                    TongTien = tien.TongTien, PhuongThucThanhToan = model.PhuongThucThanhToan,
                    GhiChu = model.GhiChu?.Trim(), NgayThanhToan = DateTime.Now,
                    TrangThaiThanhToan = "Đã thanh toán"
                });
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                TempData["SuccessMessage"] = $"Thanh toán đơn #{model.MaDatSan} thành công.";
                return RedirectToAction(nameof(ChoThanhToan));
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                return BaoLoi(ex.Message);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                return BaoLoi("Không thể lưu hóa đơn. Đơn có thể đã được thanh toán; vui lòng kiểm tra lại danh sách.");
            }
        }

        private IActionResult BaoLoi(string message)
        {
            TempData["ErrorMessage"] = message;
            return RedirectToAction(nameof(ChoThanhToan));
        }

        private async Task<IActionResult> HienHoaDonAsync(ThanhToanCreateViewModel model)
        {
            var don = await _context.DatSans.AsNoTracking().Include(d => d.KhachHang)
                .Include(d => d.SanTheThao).FirstOrDefaultAsync(d => d.MaDatSan == model.MaDatSan);
            if (don == null) return BaoLoi("Không tìm thấy đơn đặt sân.");
            if (await _context.ThanhToans.AnyAsync(t => t.MaDatSan == model.MaDatSan))
                return BaoLoi("Đơn đã có hóa đơn/đã thanh toán.");
            if (don.TrangThai != TrangThaiChoThanhToan)
                return BaoLoi("Chỉ có thể thanh toán đơn ở trạng thái Chờ thanh toán.");
            try
            {
                ViewData["DonDatSan"] = don;
                ViewData["TinhTien"] = await _tinhTien.TinhTatCaAsync(model.MaDatSan);
                return View("Create", model);
            }
            catch (InvalidOperationException ex) { return BaoLoi(ex.Message); }
        }

        public async Task<IActionResult> Index(string? tuKhoa, string? phuongThuc, string? sortOrder, int pageIndex = 1, int pageSize = 8)
        {
            if (pageSize < 1 || pageSize > 50) pageSize = 8;
            var query = _context.ThanhToans.AsNoTracking()
                .Where(t => t.TrangThaiThanhToan == "Đã thanh toán");
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim();
                query = query.Where(t => t.DatSan != null && t.DatSan.KhachHang != null
                    && t.DatSan.KhachHang.HoTen.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(phuongThuc))
                query = query.Where(t => t.PhuongThucThanhToan == phuongThuc);
            query = sortOrder switch
            {
                "ngay_asc" => query.OrderBy(t => t.NgayThanhToan).ThenBy(t => t.MaThanhToan),
                "tien_asc" => query.OrderBy(t => t.TongTien).ThenBy(t => t.MaThanhToan),
                "tien_desc" => query.OrderByDescending(t => t.TongTien).ThenByDescending(t => t.MaThanhToan),
                "khach" => query.OrderBy(t => t.DatSan!.KhachHang!.HoTen).ThenBy(t => t.MaThanhToan),
                _ => query.OrderByDescending(t => t.NgayThanhToan).ThenByDescending(t => t.MaThanhToan)
            };
            pageIndex = Math.Clamp(pageIndex, 1, Math.Max(1, (int)Math.Ceiling(await query.CountAsync() / (double)pageSize)));
            return View(new ThanhToanIndexViewModel
            {
                DanhSach = await PaginatedList<ThanhToan>.CreateAsync(query.Include(t => t.DatSan)!
                    .ThenInclude(d => d!.KhachHang).Include(t => t.DatSan)!.ThenInclude(d => d!.SanTheThao), pageIndex, pageSize),
                TuKhoa = tuKhoa, PhuongThuc = phuongThuc, SortOrder = sortOrder, PageSize = pageSize
            });
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

            pageIndex = Math.Clamp(pageIndex, 1, Math.Max(1, (int)Math.Ceiling(await danhSach.CountAsync() / (double)pageSize)));
            var viewModel = new ChoThanhToanIndexViewModel
            {
                DanhSachDon = await PaginatedList<DonChoThanhToanViewModel>.CreateAsync(danhSach, pageIndex, pageSize),
                TuKhoa = tuKhoa
            };

            return View(viewModel);
        }
    }
}
