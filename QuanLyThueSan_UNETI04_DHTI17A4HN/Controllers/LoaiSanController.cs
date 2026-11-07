// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Controller Quản lý Loại sân (CRUD, tìm kiếm, lọc, phân trang, đổi trạng thái) (Mục 5.4 file Word)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    [PhanQuyen("Admin", "NhanVien")]
    public class LoaiSanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoaiSanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================================
        // GET: /LoaiSan/Index
        // Nghiệp vụ: Danh sách loại sân có tìm kiếm, lọc trạng thái, đếm số sân và phân trang (Mục 5.4)
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, bool? trangThaiFilter, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.LoaiSans
                .Include(l => l.SanTheThaos)
                .AsNoTracking()
                .AsQueryable();

            // Tìm kiếm LINQ theo tên loại sân hoặc mô tả
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(l => l.TenLoaiSan.ToLower().Contains(keyword)
                                      || (l.MoTa != null && l.MoTa.ToLower().Contains(keyword)));
            }

            // Lọc theo trạng thái hoạt động
            if (trangThaiFilter.HasValue)
            {
                query = query.Where(l => l.TrangThai == trangThaiFilter.Value);
            }

            // Sắp xếp mặc định theo Tên loại sân
            query = query.OrderBy(l => l.TenLoaiSan);

            // Thống kê nhanh
            var tongSo = await _context.LoaiSans.CountAsync();
            var hoatDong = await _context.LoaiSans.CountAsync(l => l.TrangThai);
            var tamNgung = tongSo - hoatDong;

            // Phân trang bằng LINQ Skip / Take thông qua PaginatedList
            var pagedData = await PaginatedList<LoaiSan>.CreateAsync(query, pageIndex, pageSize);

            var danhSachTrangThai = new List<SelectListItem>
            {
                new() { Value = "", Text = "Tất cả trạng thái", Selected = !trangThaiFilter.HasValue },
                new() { Value = "true", Text = "Đang hoạt động", Selected = trangThaiFilter == true },
                new() { Value = "false", Text = "Ngừng hoạt động", Selected = trangThaiFilter == false }
            };

            var viewModel = new LoaiSanIndexViewModel
            {
                DanhSachLoaiSan = pagedData,
                TuKhoa = tuKhoa,
                TrangThaiFilter = trangThaiFilter,
                TongSoLoaiSan = tongSo,
                SoLoaiSanHoatDong = hoatDong,
                SoLoaiSanTamNgung = tamNgung,
                DanhSachTrangThai = new SelectList(danhSachTrangThai, "Value", "Text", trangThaiFilter?.ToString().ToLower())
            };

            return View(viewModel);
        }

        // =========================================================================================
        // GET: /LoaiSan/Create
        // =========================================================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new LoaiSanFormViewModel());
        }

        // =========================================================================================
        // POST: /LoaiSan/Create
        // Ràng buộc: Tên loại sân bắt buộc và không trùng. Số người tối đa > 0 nếu áp dụng (Mục 5.4, 13)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LoaiSanFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenLoaiSan))
            {
                ModelState.AddModelError(nameof(model.TenLoaiSan), "Tên loại sân không được để trống.");
            }
            else
            {
                var tenLoai = model.TenLoaiSan.Trim();
                if (await _context.LoaiSans.AnyAsync(l => l.TenLoaiSan.ToLower() == tenLoai.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.TenLoaiSan), "Tên loại sân đã tồn tại, vui lòng chọn tên khác.");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            var loaiSan = new LoaiSan
            {
                TenLoaiSan = model.TenLoaiSan.Trim(),
                MoTa = model.MoTa?.Trim(),
                SoNguoiToiDa = model.SoNguoiToiDa,
                TrangThai = model.TrangThai
            };

            _context.LoaiSans.Add(loaiSan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã thêm loại sân \"{loaiSan.TenLoaiSan}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // GET: /LoaiSan/Edit/{id}
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loaiSan = await _context.LoaiSans.FindAsync(id);
            if (loaiSan == null) return NotFound();

            var vm = new LoaiSanFormViewModel
            {
                MaLoaiSan = loaiSan.MaLoaiSan,
                TenLoaiSan = loaiSan.TenLoaiSan,
                MoTa = loaiSan.MoTa,
                SoNguoiToiDa = loaiSan.SoNguoiToiDa,
                TrangThai = loaiSan.TrangThai
            };

            return View(vm);
        }

        // =========================================================================================
        // POST: /LoaiSan/Edit/{id}
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LoaiSanFormViewModel model)
        {
            if (id != model.MaLoaiSan) return BadRequest();

            if (string.IsNullOrWhiteSpace(model.TenLoaiSan))
            {
                ModelState.AddModelError(nameof(model.TenLoaiSan), "Tên loại sân không được để trống.");
            }
            else
            {
                var tenLoai = model.TenLoaiSan.Trim();
                if (await _context.LoaiSans.AnyAsync(l => l.MaLoaiSan != id && l.TenLoaiSan.ToLower() == tenLoai.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.TenLoaiSan), "Tên loại sân đã tồn tại trên hệ thống.");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            var loaiSan = await _context.LoaiSans.FindAsync(id);
            if (loaiSan == null) return NotFound();

            loaiSan.TenLoaiSan = model.TenLoaiSan.Trim();
            loaiSan.MoTa = model.MoTa?.Trim();
            loaiSan.SoNguoiToiDa = model.SoNguoiToiDa;
            loaiSan.TrangThai = model.TrangThai;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cập nhật loại sân \"{loaiSan.TenLoaiSan}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // GET: /LoaiSan/Details/{id}
        // Nghiệp vụ: Xem chi tiết loại sân kèm danh sách các sân đang áp dụng loại sân này (Mục 5.4, 22.2)
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loaiSan = await _context.LoaiSans
                .Include(l => l.SanTheThaos)
                    .ThenInclude(s => s.KhuVuc)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.MaLoaiSan == id);

            if (loaiSan == null) return NotFound();

            return View(loaiSan);
        }

        // =========================================================================================
        // POST: /LoaiSan/DoiTrangThai/{id}
        // Nghiệp vụ: Bật / Tắt trạng thái hoạt động của loại sân (Mục 5.4: loại sân ngừng hoạt động không được dùng cho sân mới)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var loaiSan = await _context.LoaiSans
                .Include(l => l.SanTheThaos)
                .FirstOrDefaultAsync(l => l.MaLoaiSan == id);

            if (loaiSan == null) return NotFound();

            loaiSan.TrangThai = !loaiSan.TrangThai;
            await _context.SaveChangesAsync();

            var trangThaiText = loaiSan.TrangThai ? "Đang hoạt động" : "Ngừng hoạt động";
            TempData["SuccessMessage"] = $"Đã chuyển trạng thái loại sân \"{loaiSan.TenLoaiSan}\" sang: {trangThaiText}.";

            return RedirectToAction(nameof(Index));
        }
    }
}
