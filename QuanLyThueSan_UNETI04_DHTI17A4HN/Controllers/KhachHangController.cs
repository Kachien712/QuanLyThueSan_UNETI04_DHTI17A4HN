// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 3 - Controller Quản lý Khách hàng (CRUD, Tìm kiếm, Lọc, Phân trang LINQ, Khóa/Mở khóa)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhachHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. DANH SÁCH KHÁCH HÀNG (Tìm kiếm + Lọc + Sắp xếp + Phân trang LINQ)
        public async Task<IActionResult> Index(string? tuKhoa, bool? trangThaiFilter, string? sortOrder, int pageIndex = 1, int pageSize = 8)
        {
            var query = _context.KhachHangs.AsNoTracking().AsQueryable();

            // Tìm kiếm theo Họ tên, Số điện thoại hoặc Email
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim();
                query = query.Where(k => k.HoTen.Contains(keyword) 
                                      || k.SoDienThoai.Contains(keyword) 
                                      || (k.Email != null && k.Email.Contains(keyword)));
            }

            // Lọc theo trạng thái hoạt động
            if (trangThaiFilter.HasValue)
            {
                query = query.Where(k => k.TrangThai == trangThaiFilter.Value);
            }

            // Sắp xếp
            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(k => k.HoTen),
                "name_asc" => query.OrderBy(k => k.HoTen),
                "phone" => query.OrderBy(k => k.SoDienThoai),
                _ => query.OrderByDescending(k => k.MaKhachHang) // Mặc định: mới nhất lên đầu
            };

            // Thống kê nhanh
            var tongSo = await _context.KhachHangs.CountAsync();
            var hoatDong = await _context.KhachHangs.CountAsync(k => k.TrangThai);
            var biKhoa = tongSo - hoatDong;

            // Thực hiện phân trang LINQ bằng Skip() và Take() qua PaginatedList
            var pagedData = await PaginatedList<KhachHang>.CreateAsync(query, pageIndex, pageSize);

            var viewModel = new KhachHangIndexViewModel
            {
                DanhSachKhachHang = pagedData,
                TuKhoa = tuKhoa,
                TrangThaiFilter = trangThaiFilter,
                SortOrder = sortOrder,
                TongSoKhachHang = tongSo,
                SoKhachHangHoatDong = hoatDong,
                SoKhachHangBiKhoa = biKhoa,
                DanhSachTrangThai = new SelectList(new[]
                {
                    new { Value = "", Text = "-- Tất cả trạng thái --" },
                    new { Value = "true", Text = "Đang hoạt động" },
                    new { Value = "false", Text = "Bị khóa" }
                }, "Value", "Text", trangThaiFilter?.ToString().ToLower())
            };

            return View(viewModel);
        }

        // 2. CHI TIẾT KHÁCH HÀNG VÀ LỊCH SỬ ĐẶT SÂN
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs
                .Include(k => k.DatSans)
                    .ThenInclude(d => d.SanTheThao)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaKhachHang == id);

            if (khachHang == null)
            {
                return NotFound();
            }

            return View(khachHang);
        }

        // 3. THÊM MỚI KHÁCH HÀNG (GET)
        public IActionResult Create()
        {
            var model = new KhachHangFormViewModel();
            return View(model);
        }

        // 3. THÊM MỚI KHÁCH HÀNG (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachHangFormViewModel model)
        {
            // Kiểm tra trùng lặp số điện thoại
            if (await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == model.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }

            // Kiểm tra trùng lặp email (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(model.Email) && await _context.KhachHangs.AnyAsync(k => k.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng bởi khách hàng khác.");
            }

            if (ModelState.IsValid)
            {
                var khachHang = new KhachHang
                {
                    HoTen = model.HoTen.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim(),
                    Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim(),
                    DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi.Trim(),
                    TrangThai = model.TrangThai
                };

                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm khách hàng mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 4. CHỈNH SỬA THÔNG TIN KHÁCH HÀNG (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            var model = new KhachHangFormViewModel
            {
                MaKhachHang = khachHang.MaKhachHang,
                MaTaiKhoan = khachHang.MaTaiKhoan,
                HoTen = khachHang.HoTen,
                SoDienThoai = khachHang.SoDienThoai,
                Email = khachHang.Email,
                DiaChi = khachHang.DiaChi,
                TrangThai = khachHang.TrangThai
            };

            return View(model);
        }

        // 4. CHỈNH SỬA THÔNG TIN KHÁCH HÀNG (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachHangFormViewModel model)
        {
            if (id != model.MaKhachHang)
            {
                return NotFound();
            }

            // Kiểm tra trùng số điện thoại với khách hàng khác
            if (await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == model.SoDienThoai && k.MaKhachHang != id))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng bởi khách hàng khác.");
            }

            // Kiểm tra trùng email với khách hàng khác
            if (!string.IsNullOrWhiteSpace(model.Email) && 
                await _context.KhachHangs.AnyAsync(k => k.Email == model.Email && k.MaKhachHang != id))
            {
                ModelState.AddModelError("Email", "Địa chỉ email này đã được sử dụng bởi khách hàng khác.");
            }

            if (ModelState.IsValid)
            {
                var khachHang = await _context.KhachHangs.FindAsync(id);
                if (khachHang == null)
                {
                    return NotFound();
                }

                khachHang.HoTen = model.HoTen.Trim();
                khachHang.SoDienThoai = model.SoDienThoai.Trim();
                khachHang.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
                khachHang.DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi.Trim();
                khachHang.TrangThai = model.TrangThai;

                _context.Update(khachHang);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 5. KHÓA / MỞ KHÓA TÀI KHOẢN KHÁCH HÀNG (Thay thế xóa vật lý để giữ lịch sử đặt sân)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            var khachHang = await _context.KhachHangs.FindAsync(id);
            if (khachHang == null)
            {
                return NotFound();
            }

            khachHang.TrangThai = !khachHang.TrangThai;
            _context.Update(khachHang);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = khachHang.TrangThai 
                ? $"Đã mở khóa hoạt động cho khách hàng {khachHang.HoTen}." 
                : $"Đã khóa khách hàng {khachHang.HoTen}. Khách hàng bị khóa sẽ không thể đặt sân.";

            return RedirectToAction(nameof(Index));
        }
    }
}
