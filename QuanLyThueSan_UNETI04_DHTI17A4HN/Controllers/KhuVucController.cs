// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Controller Quản lý Khu vực (CRUD, tìm kiếm, lọc, phân trang, đổi trạng thái) (Mục 5.5 file Word)

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
    public class KhuVucController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhuVucController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================================
        // GET: /KhuVuc/Index
        // Nghiệp vụ: Danh sách khu vực có tìm kiếm, lọc trạng thái, đếm số sân và phân trang (Mục 5.5)
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? tuKhoa, bool? trangThaiFilter, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.KhuVucs
                .Include(k => k.SanTheThaos)
                .AsNoTracking()
                .AsQueryable();

            // Tìm kiếm LINQ theo tên khu vực, vị trí hoặc mô tả
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(k => k.TenKhuVuc.ToLower().Contains(keyword)
                                      || (k.ViTri != null && k.ViTri.ToLower().Contains(keyword))
                                      || (k.MoTa != null && k.MoTa.ToLower().Contains(keyword)));
            }

            // Lọc theo trạng thái hoạt động
            if (trangThaiFilter.HasValue)
            {
                query = query.Where(k => k.TrangThai == trangThaiFilter.Value);
            }

            // Sắp xếp mặc định theo Tên khu vực
            query = query.OrderBy(k => k.TenKhuVuc);

            // Thống kê nhanh
            var tongSo = await _context.KhuVucs.CountAsync();
            var hoatDong = await _context.KhuVucs.CountAsync(k => k.TrangThai);
            var tamNgung = tongSo - hoatDong;

            // Phân trang bằng LINQ Skip / Take
            var pagedData = await PaginatedList<KhuVuc>.CreateAsync(query, pageIndex, pageSize);

            var danhSachTrangThai = new List<SelectListItem>
            {
                new() { Value = "", Text = "Tất cả trạng thái", Selected = !trangThaiFilter.HasValue },
                new() { Value = "true", Text = "Đang hoạt động", Selected = trangThaiFilter == true },
                new() { Value = "false", Text = "Ngừng hoạt động", Selected = trangThaiFilter == false }
            };

            var viewModel = new KhuVucIndexViewModel
            {
                DanhSachKhuVuc = pagedData,
                TuKhoa = tuKhoa,
                TrangThaiFilter = trangThaiFilter,
                TongSoKhuVuc = tongSo,
                SoKhuVucHoatDong = hoatDong,
                SoKhuVucTamNgung = tamNgung,
                DanhSachTrangThai = new SelectList(danhSachTrangThai, "Value", "Text", trangThaiFilter?.ToString().ToLower())
            };

            return View(viewModel);
        }

        // =========================================================================================
        // GET: /KhuVuc/Create
        // =========================================================================================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new KhuVucFormViewModel());
        }

        // =========================================================================================
        // POST: /KhuVuc/Create
        // Ràng buộc: Tên khu vực bắt buộc và không trùng theo quy định (Mục 5.5, 13)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhuVucFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.TenKhuVuc))
            {
                ModelState.AddModelError(nameof(model.TenKhuVuc), "Tên khu vực không được để trống.");
            }
            else
            {
                var tenKhuVuc = model.TenKhuVuc.Trim();
                if (await _context.KhuVucs.AnyAsync(k => k.TenKhuVuc.ToLower() == tenKhuVuc.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.TenKhuVuc), "Tên khu vực đã tồn tại, vui lòng chọn tên khác.");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            var khuVuc = new KhuVuc
            {
                TenKhuVuc = model.TenKhuVuc.Trim(),
                ViTri = model.ViTri?.Trim(),
                MoTa = model.MoTa?.Trim(),
                TrangThai = model.TrangThai
            };

            _context.KhuVucs.Add(khuVuc);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã thêm khu vực \"{khuVuc.TenKhuVuc}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // GET: /KhuVuc/Edit/{id}
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var khuVuc = await _context.KhuVucs.FindAsync(id);
            if (khuVuc == null) return NotFound();

            var vm = new KhuVucFormViewModel
            {
                MaKhuVuc = khuVuc.MaKhuVuc,
                TenKhuVuc = khuVuc.TenKhuVuc,
                ViTri = khuVuc.ViTri,
                MoTa = khuVuc.MoTa,
                TrangThai = khuVuc.TrangThai
            };

            return View(vm);
        }

        // =========================================================================================
        // POST: /KhuVuc/Edit/{id}
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhuVucFormViewModel model)
        {
            if (id != model.MaKhuVuc) return BadRequest();

            if (string.IsNullOrWhiteSpace(model.TenKhuVuc))
            {
                ModelState.AddModelError(nameof(model.TenKhuVuc), "Tên khu vực không được để trống.");
            }
            else
            {
                var tenKhuVuc = model.TenKhuVuc.Trim();
                if (await _context.KhuVucs.AnyAsync(k => k.MaKhuVuc != id && k.TenKhuVuc.ToLower() == tenKhuVuc.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.TenKhuVuc), "Tên khu vực đã tồn tại trên hệ thống.");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            var khuVuc = await _context.KhuVucs.FindAsync(id);
            if (khuVuc == null) return NotFound();

            khuVuc.TenKhuVuc = model.TenKhuVuc.Trim();
            khuVuc.ViTri = model.ViTri?.Trim();
            khuVuc.MoTa = model.MoTa?.Trim();
            khuVuc.TrangThai = model.TrangThai;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cập nhật khu vực \"{khuVuc.TenKhuVuc}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // GET: /KhuVuc/Details/{id}
        // Nghiệp vụ: Xem chi tiết khu vực kèm danh sách các sân nằm trong khu vực này (Mục 5.5, 22.2)
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var khuVuc = await _context.KhuVucs
                .Include(k => k.SanTheThaos)
                    .ThenInclude(s => s.LoaiSan)
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhuVuc == id);

            if (khuVuc == null) return NotFound();

            return View(khuVuc);
        }

        // =========================================================================================
        // POST: /KhuVuc/DoiTrangThai/{id}
        // Nghiệp vụ: Bật / Tắt trạng thái hoạt động của khu vực (Mục 5.5: khu vực ngừng hoạt động không được gán cho sân mới)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var khuVuc = await _context.KhuVucs
                .Include(k => k.SanTheThaos)
                .FirstOrDefaultAsync(k => k.MaKhuVuc == id);

            if (khuVuc == null) return NotFound();

            khuVuc.TrangThai = !khuVuc.TrangThai;
            await _context.SaveChangesAsync();

            var trangThaiText = khuVuc.TrangThai ? "Đang hoạt động" : "Ngừng hoạt động";
            TempData["SuccessMessage"] = $"Đã chuyển trạng thái khu vực \"{khuVuc.TenKhuVuc}\" sang: {trangThaiText}.";

            return RedirectToAction(nameof(Index));
        }
    }
}
