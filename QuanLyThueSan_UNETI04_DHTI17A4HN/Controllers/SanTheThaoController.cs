// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - Controller quản lý Sân thể thao (Tìm kiếm, Lọc, Sắp xếp, Phân trang, Thêm mới, Chi tiết, Chỉnh sửa)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    public class SanTheThaoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SanTheThaoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================================
        // GET: /SanTheThao/Index
        // Nghiệp vụ: Tìm kiếm + Lọc đa tiêu chí + Sắp xếp + Phân trang bằng LINQ (Mục 6.4 - 6.7)
        // Luồng: Truy vấn -> Tìm kiếm -> Lọc -> Sắp xếp -> Phân trang -> Hiển thị
        // =========================================================================================
        public async Task<IActionResult> Index(
            string? tuKhoa,
            int? maLoaiSan,
            int? maKhuVuc,
            string? tinhTrang,
            int? sucChuaToiThieu,
            string? sortOrder,
            int pageIndex = 1)
        {
            // 1. Khởi tạo truy vấn gốc kết hợp nạp dữ liệu liên quan (Eager Loading)
            var query = _context.SanTheThaos
                .Include(s => s.LoaiSan)
                .Include(s => s.KhuVuc)
                .Include(s => s.BangGias)
                .AsNoTracking()
                .AsQueryable();

            // 2. Tìm kiếm LINQ (Mục 6.4: Tên sân, Tên loại sân, Tên khu vực)
            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(s => s.TenSan.ToLower().Contains(keyword)
                                      || (s.LoaiSan != null && s.LoaiSan.TenLoaiSan.ToLower().Contains(keyword))
                                      || (s.KhuVuc != null && s.KhuVuc.TenKhuVuc.ToLower().Contains(keyword)));
            }

            // 3. Lọc LINQ đa tiêu chí (Mục 6.5)
            // Lọc theo Loại sân
            if (maLoaiSan.HasValue && maLoaiSan.Value > 0)
            {
                query = query.Where(s => s.MaLoaiSan == maLoaiSan.Value);
            }

            // Lọc theo Khu vực
            if (maKhuVuc.HasValue && maKhuVuc.Value > 0)
            {
                query = query.Where(s => s.MaKhuVuc == maKhuVuc.Value);
            }

            // Lọc theo Tình trạng sân (Đang hoạt động, Tạm ngừng, Bảo trì...)
            if (!string.IsNullOrWhiteSpace(tinhTrang))
            {
                query = query.Where(s => s.TinhTrang == tinhTrang);
            }

            // Lọc theo Sức chứa tối thiểu
            if (sucChuaToiThieu.HasValue && sucChuaToiThieu.Value > 0)
            {
                query = query.Where(s => s.SucChua >= sucChuaToiThieu.Value);
            }

            // 4. Sắp xếp LINQ (Mục 6.6)
            query = sortOrder switch
            {
                "ten_desc" => query.OrderByDescending(s => s.TenSan),
                "succhua_asc" => query.OrderBy(s => s.SucChua).ThenBy(s => s.TenSan),
                "succhua_desc" => query.OrderByDescending(s => s.SucChua).ThenBy(s => s.TenSan),
                "loai_asc" => query.OrderBy(s => s.LoaiSan != null ? s.LoaiSan.TenLoaiSan : "").ThenBy(s => s.TenSan),
                "khuvuc_asc" => query.OrderBy(s => s.KhuVuc != null ? s.KhuVuc.TenKhuVuc : "").ThenBy(s => s.TenSan),
                _ => query.OrderBy(s => s.TenSan) // Mặc định: Tên sân A -> Z
            };

            // 5. Phân trang LINQ Skip() & Take() qua PaginatedList (Mục 6.7)
            const int pageSize = 10;
            if (pageIndex < 1) pageIndex = 1;
            var pagedSanTheThao = await PaginatedList<SanTheThao>.CreateAsync(query, pageIndex, pageSize);

            // 6. Chuẩn bị dữ liệu danh sách cho các dropdown lọc trên giao diện
            var loaiSans = await _context.LoaiSans
                .Where(l => l.TrangThai)
                .OrderBy(l => l.TenLoaiSan)
                .AsNoTracking()
                .ToListAsync();

            var khuVucs = await _context.KhuVucs
                .Where(k => k.TrangThai)
                .OrderBy(k => k.TenKhuVuc)
                .AsNoTracking()
                .ToListAsync();

            var danhSachTinhTrang = new List<string>
            {
                "Đang hoạt động",
                "Tạm ngừng",
                "Đang bảo trì",
                "Ngừng sử dụng"
            };

            // 7. Đóng gói toàn bộ vào ViewModel
            var viewModel = new SanTheThaoIndexViewModel
            {
                DanhSachSan = pagedSanTheThao,
                TuKhoa = tuKhoa,
                MaLoaiSan = maLoaiSan,
                MaKhuVuc = maKhuVuc,
                TinhTrang = tinhTrang,
                SucChuaToiThieu = sucChuaToiThieu,
                SortOrder = sortOrder,
                TrangHienTai = pagedSanTheThao.TrangHienTai,
                TongSoTrang = pagedSanTheThao.TongSoTrang,
                TongSoSan = pagedSanTheThao.TongSoBanGhi,
                KichThuocTrang = pageSize,
                DanhSachLoaiSan = new SelectList(loaiSans, "MaLoaiSan", "TenLoaiSan", maLoaiSan),
                DanhSachKhuVuc = new SelectList(khuVucs, "MaKhuVuc", "TenKhuVuc", maKhuVuc),
                DanhSachTinhTrang = new SelectList(danhSachTinhTrang, tinhTrang)
            };

            return View(viewModel);
        }

        // =========================================================================================
        // GET: /SanTheThao/Create
        // Nghiệp vụ: Hiển thị form thêm mới sân thể thao (Mục 6.3 file Word)
        // =========================================================================================
        public async Task<IActionResult> Create()
        {
            var vm = new SanTheThaoFormViewModel
            {
                DanhSachLoaiSan = new SelectList(
                    await _context.LoaiSans
                        .Where(l => l.TrangThai)
                        .OrderBy(l => l.TenLoaiSan)
                        .AsNoTracking()
                        .ToListAsync(),
                    "MaLoaiSan", "TenLoaiSan"),

                DanhSachKhuVuc = new SelectList(
                    await _context.KhuVucs
                        .Where(k => k.TrangThai)
                        .OrderBy(k => k.TenKhuVuc)
                        .AsNoTracking()
                        .ToListAsync(),
                    "MaKhuVuc", "TenKhuVuc")
            };

            return View(vm);
        }

        // =========================================================================================
        // POST: /SanTheThao/Create
        // Nghiệp vụ: Tiếp nhận form, validate, kiểm tra tên trùng, lưu sân mới vào DB (Mục 6.3, 13)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanTheThaoFormViewModel vm)
        {
            // Kiểm tra tên sân không được trùng (Mục 13 file Word)
            bool tenBiTrung = await _context.SanTheThaos
                .AnyAsync(s => s.TenSan.ToLower() == vm.TenSan.Trim().ToLower());

            if (tenBiTrung)
            {
                ModelState.AddModelError(nameof(vm.TenSan), "Tên sân này đã tồn tại. Vui lòng nhập tên khác.");
            }

            if (!ModelState.IsValid)
            {
                // Nạp lại dropdown khi form lỗi
                vm.DanhSachLoaiSan = new SelectList(
                    await _context.LoaiSans.Where(l => l.TrangThai).OrderBy(l => l.TenLoaiSan).AsNoTracking().ToListAsync(),
                    "MaLoaiSan", "TenLoaiSan", vm.MaLoaiSan);

                vm.DanhSachKhuVuc = new SelectList(
                    await _context.KhuVucs.Where(k => k.TrangThai).OrderBy(k => k.TenKhuVuc).AsNoTracking().ToListAsync(),
                    "MaKhuVuc", "TenKhuVuc", vm.MaKhuVuc);

                return View(vm);
            }

            var sanMoi = new SanTheThao
            {
                TenSan     = vm.TenSan.Trim(),
                MaLoaiSan  = vm.MaLoaiSan,
                MaKhuVuc   = vm.MaKhuVuc,
                SucChua    = vm.SucChua,
                MoTa       = vm.MoTa?.Trim(),
                TinhTrang  = vm.TinhTrang
            };

            _context.SanTheThaos.Add(sanMoi);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã thêm sân \"{sanMoi.TenSan}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // GET: /SanTheThao/Details/{id}
        // Nghiệp vụ: Xem chi tiết thông tin sân thể thao và danh sách bảng giá áp dụng (Mục 6.3 file Word)
        // =========================================================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanTheThao = await _context.SanTheThaos
                .Include(s => s.LoaiSan)
                .Include(s => s.KhuVuc)
                .Include(s => s.BangGias.OrderBy(b => b.GioBatDau))
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MaSan == id);

            if (sanTheThao == null)
            {
                return NotFound();
            }

            return View(sanTheThao);
        }

        // =========================================================================================
        // GET: /SanTheThao/Edit/{id}
        // Nghiệp vụ: Hiển thị form chỉnh sửa thông tin sân thể thao (Mục 6.3 file Word)
        // =========================================================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var san = await _context.SanTheThaos.FindAsync(id);
            if (san == null)
            {
                return NotFound();
            }

            var vm = new SanTheThaoFormViewModel
            {
                MaSan = san.MaSan,
                TenSan = san.TenSan,
                MaLoaiSan = san.MaLoaiSan,
                MaKhuVuc = san.MaKhuVuc,
                SucChua = san.SucChua,
                MoTa = san.MoTa,
                TinhTrang = san.TinhTrang,

                DanhSachLoaiSan = new SelectList(
                    await _context.LoaiSans
                        .Where(l => l.TrangThai || l.MaLoaiSan == san.MaLoaiSan)
                        .OrderBy(l => l.TenLoaiSan)
                        .AsNoTracking()
                        .ToListAsync(),
                    "MaLoaiSan", "TenLoaiSan", san.MaLoaiSan),

                DanhSachKhuVuc = new SelectList(
                    await _context.KhuVucs
                        .Where(k => k.TrangThai || k.MaKhuVuc == san.MaKhuVuc)
                        .OrderBy(k => k.TenKhuVuc)
                        .AsNoTracking()
                        .ToListAsync(),
                    "MaKhuVuc", "TenKhuVuc", san.MaKhuVuc)
            };

            return View(vm);
        }

        // =========================================================================================
        // POST: /SanTheThao/Edit/{id}
        // Nghiệp vụ: Xác thực dữ liệu, kiểm tra tên trùng (trừ chính nó) và cập nhật thông tin sân (Mục 6.3, 13)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SanTheThaoFormViewModel vm)
        {
            if (id != vm.MaSan)
            {
                return NotFound();
            }

            // Kiểm tra tên sân không được trùng với sân khác (Mục 13 file Word)
            bool tenBiTrung = await _context.SanTheThaos
                .AnyAsync(s => s.TenSan.ToLower() == vm.TenSan.Trim().ToLower() && s.MaSan != id);

            if (tenBiTrung)
            {
                ModelState.AddModelError(nameof(vm.TenSan), "Tên sân này đã được sử dụng bởi một sân khác. Vui lòng nhập tên khác.");
            }

            if (!ModelState.IsValid)
            {
                vm.DanhSachLoaiSan = new SelectList(
                    await _context.LoaiSans.Where(l => l.TrangThai || l.MaLoaiSan == vm.MaLoaiSan).OrderBy(l => l.TenLoaiSan).AsNoTracking().ToListAsync(),
                    "MaLoaiSan", "TenLoaiSan", vm.MaLoaiSan);

                vm.DanhSachKhuVuc = new SelectList(
                    await _context.KhuVucs.Where(k => k.TrangThai || k.MaKhuVuc == vm.MaKhuVuc).OrderBy(k => k.TenKhuVuc).AsNoTracking().ToListAsync(),
                    "MaKhuVuc", "TenKhuVuc", vm.MaKhuVuc);

                return View(vm);
            }

            var sanCanSua = await _context.SanTheThaos.FindAsync(id);
            if (sanCanSua == null)
            {
                return NotFound();
            }

            // Cập nhật các trường thông tin
            sanCanSua.TenSan = vm.TenSan.Trim();
            sanCanSua.MaLoaiSan = vm.MaLoaiSan;
            sanCanSua.MaKhuVuc = vm.MaKhuVuc;
            sanCanSua.SucChua = vm.SucChua;
            sanCanSua.MoTa = vm.MoTa?.Trim();
            sanCanSua.TinhTrang = vm.TinhTrang;

            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã cập nhật thông tin sân \"{sanCanSua.TenSan}\" thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
