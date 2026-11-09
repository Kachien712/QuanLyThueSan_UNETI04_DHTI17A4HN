// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - Quản lý Sân thể thao & Bảng giá
// - Cơ bản: Tìm kiếm, Lọc đa tiêu chí, Sắp xếp (Mục 6.6), Phân trang (Mục 6.7), CRUD Sân, Phân quyền Admin/NhanVien (Mục 6.3)
// - Nâng cao (Mục 19.1): Upload ảnh đại diện sân thể thao (lưu trữ wwwroot/images/san, validate định dạng & dung lượng, xóa file cũ)
// - Nâng cao (Mục 19.6): AJAX Live Search & Filter (trả PartialView _DanhSachSanPartial cho request AJAX/fetch không cần tải lại trang)
// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Nạp lịch sử sử dụng trong chi tiết sân

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
    public class SanTheThaoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SanTheThaoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // =========================================================================================
        // GET: /SanTheThao/Index
        // Nghiệp vụ: Tìm kiếm + Lọc đa tiêu chí + Sắp xếp + Phân trang bằng LINQ (Mục 6.4 - 6.7)
        // Chức năng nâng cao: Hỗ trợ AJAX Live Search & Filter (Mục 19.6)
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

            // 4. Sắp xếp LINQ (Mục 6.6: Tên sân A-Z, Z-A; Sức chứa tăng/giảm; Đơn giá tăng/giảm; Loại sân; Khu vực)
            query = sortOrder switch
            {
                "ten_desc" => query.OrderByDescending(s => s.TenSan),
                "dongia_asc" => query.OrderBy(s => s.BangGias.Where(b => b.TrangThai).OrderBy(b => b.DonGiaMoiGio).Select(b => (decimal?)b.DonGiaMoiGio).FirstOrDefault() ?? 0).ThenBy(s => s.TenSan),
                "dongia_desc" => query.OrderByDescending(s => s.BangGias.Where(b => b.TrangThai).OrderByDescending(b => b.DonGiaMoiGio).Select(b => (decimal?)b.DonGiaMoiGio).FirstOrDefault() ?? 0).ThenBy(s => s.TenSan),
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

            // Chức năng nâng cao: Kiểm tra request AJAX (Mục 19.6)
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                       || string.Equals(Request.Query["isAjax"], "true", StringComparison.OrdinalIgnoreCase);

            if (isAjax)
            {
                return PartialView("_DanhSachSanPartial", viewModel);
            }

            return View(viewModel);
        }

        // =========================================================================================
        // GET: /SanTheThao/Create
        // Nghiệp vụ: Hiển thị form thêm mới sân thể thao (Mục 6.3 file Word)
        // Phân quyền: Chỉ Admin và Nhân viên quản lý mới được thêm sân (Mục 6.3)
        // =========================================================================================
        [HttpGet]
        [PhanQuyen("Admin", "NhanVien")]
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
        // Nghiệp vụ: Tiếp nhận form, validate, kiểm tra tên trùng, upload ảnh sân, lưu vào DB (Mục 6.3, 13, 19.1)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin", "NhanVien")]
        public async Task<IActionResult> Create(SanTheThaoFormViewModel vm)
        {
            // Kiểm tra tên sân không được trùng (Mục 13 file Word)
            bool tenBiTrung = await _context.SanTheThaos
                .AnyAsync(s => s.TenSan.ToLower() == vm.TenSan.Trim().ToLower());

            if (tenBiTrung)
            {
                ModelState.AddModelError(nameof(vm.TenSan), "Tên sân này đã tồn tại. Vui lòng nhập tên khác.");
            }

            // Mục 5.4 & 5.5: Loại sân / khu vực đã ngừng hoạt động không được dùng để tạo sân mới
            bool loaiSanHopLe = await _context.LoaiSans
                .AnyAsync(l => l.MaLoaiSan == vm.MaLoaiSan && l.TrangThai);
            if (!loaiSanHopLe)
            {
                ModelState.AddModelError(nameof(vm.MaLoaiSan), "Loại sân không tồn tại hoặc đã ngừng hoạt động.");
            }

            bool khuVucHopLe = await _context.KhuVucs
                .AnyAsync(k => k.MaKhuVuc == vm.MaKhuVuc && k.TrangThai);
            if (!khuVucHopLe)
            {
                ModelState.AddModelError(nameof(vm.MaKhuVuc), "Khu vực không tồn tại hoặc đã ngừng hoạt động.");
            }

            // Chức năng nâng cao: Validate file ảnh sân (Mục 19.1)
            if (vm.FileAnh != null && vm.FileAnh.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
                var ext = Path.GetExtension(vm.FileAnh.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.FileAnh), "Định dạng ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .webp, .svg");
                }
                else if (vm.FileAnh.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(vm.FileAnh), "Dung lượng file ảnh vượt quá 5MB.");
                }
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

            // Xử lý lưu ảnh nếu có upload
            string? tenFileAnh = null;
            if (vm.FileAnh != null && vm.FileAnh.Length > 0)
            {
                tenFileAnh = await LuuFileAnhAsync(vm.FileAnh);
            }

            var sanMoi = new SanTheThao
            {
                TenSan     = vm.TenSan.Trim(),
                MaLoaiSan  = vm.MaLoaiSan,
                MaKhuVuc   = vm.MaKhuVuc,
                SucChua    = vm.SucChua,
                MoTa       = vm.MoTa?.Trim(),
                TinhTrang  = vm.TinhTrang,
                HinhAnh    = tenFileAnh
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
                .Include(s => s.DatSans).ThenInclude(d => d.KhachHang)
                .Include(s => s.DatSans).ThenInclude(d => d.ThanhToan)
                .Include(s => s.DatSans).ThenInclude(d => d.SuDungDichVus).ThenInclude(d => d.DichVu)
                .AsSplitQuery()
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
        // Phân quyền: Chỉ Admin và Nhân viên quản lý mới được sửa sân (Mục 6.3)
        // =========================================================================================
        [HttpGet]
        [PhanQuyen("Admin", "NhanVien")]
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
                HinhAnhHienTai = san.HinhAnh,

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
        // Nghiệp vụ: Xác thực dữ liệu, kiểm tra tên trùng, cập nhật ảnh đại diện sân (Mục 6.3, 13, 19.1)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int id, SanTheThaoFormViewModel vm)
        {
            if (id != vm.MaSan)
            {
                return NotFound();
            }

            var sanCanSua = await _context.SanTheThaos.FindAsync(id);
            if (sanCanSua == null)
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

            // Mục 5.4 & 5.5: chỉ cho phép giữ nguyên loại sân/khu vực cũ nếu chúng đã ngừng hoạt động,
            // không được đổi sang một loại sân/khu vực ngừng hoạt động khác
            bool loaiSanHopLe = await _context.LoaiSans
                .AnyAsync(l => l.MaLoaiSan == vm.MaLoaiSan && (l.TrangThai || l.MaLoaiSan == sanCanSua.MaLoaiSan));
            if (!loaiSanHopLe)
            {
                ModelState.AddModelError(nameof(vm.MaLoaiSan), "Loại sân không tồn tại hoặc đã ngừng hoạt động.");
            }

            bool khuVucHopLe = await _context.KhuVucs
                .AnyAsync(k => k.MaKhuVuc == vm.MaKhuVuc && (k.TrangThai || k.MaKhuVuc == sanCanSua.MaKhuVuc));
            if (!khuVucHopLe)
            {
                ModelState.AddModelError(nameof(vm.MaKhuVuc), "Khu vực không tồn tại hoặc đã ngừng hoạt động.");
            }

            // Validate file ảnh nếu có upload
            if (vm.FileAnh != null && vm.FileAnh.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
                var ext = Path.GetExtension(vm.FileAnh.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError(nameof(vm.FileAnh), "Định dạng ảnh không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .webp, .svg");
                }
                else if (vm.FileAnh.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(nameof(vm.FileAnh), "Dung lượng file ảnh vượt quá 5MB.");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.HinhAnhHienTai = sanCanSua.HinhAnh;
                vm.DanhSachLoaiSan = new SelectList(
                    await _context.LoaiSans.Where(l => l.TrangThai || l.MaLoaiSan == vm.MaLoaiSan).OrderBy(l => l.TenLoaiSan).AsNoTracking().ToListAsync(),
                    "MaLoaiSan", "TenLoaiSan", vm.MaLoaiSan);

                vm.DanhSachKhuVuc = new SelectList(
                    await _context.KhuVucs.Where(k => k.TrangThai || k.MaKhuVuc == vm.MaKhuVuc).OrderBy(k => k.TenKhuVuc).AsNoTracking().ToListAsync(),
                    "MaKhuVuc", "TenKhuVuc", vm.MaKhuVuc);

                return View(vm);
            }

            // Chức năng nâng cao: Cập nhật ảnh sân mới và xóa ảnh cũ nếu có (Mục 19.1)
            if (vm.FileAnh != null && vm.FileAnh.Length > 0)
            {
                var fileAnhMoi = await LuuFileAnhAsync(vm.FileAnh);
                if (!string.IsNullOrEmpty(fileAnhMoi))
                {
                    XoaFileAnhCu(sanCanSua.HinhAnh);
                    sanCanSua.HinhAnh = fileAnhMoi;
                }
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

        // =========================================================================================
        // HÀM HỖ TRỢ XỬ LÝ UPLOAD VÀ XÓA FILE ẢNH SÂN THỂ THAO (Mục 19.1)
        // =========================================================================================
        private async Task<string?> LuuFileAnhAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".svg" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext)) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "san");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"san_{Guid.NewGuid():N}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        private void XoaFileAnhCu(string? fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;

            // Không xóa các file ảnh svg mặc định của hệ thống
            if (fileName.StartsWith("default-") || fileName.StartsWith("san-")) return;

            try
            {
                var filePath = Path.Combine(_env.WebRootPath, "images", "san", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch
            {
                // Bỏ qua nếu file đang bị lock
            }
        }
    }
}
