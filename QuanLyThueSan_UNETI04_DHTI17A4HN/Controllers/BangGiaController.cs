// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - Controller quản lý Bảng giá thuê sân theo khung giờ (Mục 6.2, 6.3, 6.8 file Word)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Controllers
{
    public class BangGiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BangGiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================================
        // GET: /BangGia/Index?maSan=...
        // Nghiệp vụ: Xem danh sách bảng giá theo từng sân thể thao hoặc toàn bộ hệ thống (Mục 6.2, 6.3)
        // Mọi người dùng (kể cả khách) đều có thể xem bảng giá
        // =========================================================================================
        public async Task<IActionResult> Index(int? maSan)
        {
            var query = _context.BangGias
                .Include(b => b.SanTheThao)
                .AsNoTracking()
                .AsQueryable();

            if (maSan.HasValue && maSan.Value > 0)
            {
                query = query.Where(b => b.MaSan == maSan.Value);
                var san = await _context.SanTheThaos.FindAsync(maSan.Value);
                ViewBag.TenSanHienThi = san?.TenSan ?? "Sân thể thao";
                ViewBag.MaSanHienThi = maSan.Value;
            }
            else
            {
                ViewBag.TenSanHienThi = "Tất cả các sân";
                ViewBag.MaSanHienThi = null;
            }

            var danhSachBangGia = await query
                .OrderBy(b => b.SanTheThao != null ? b.SanTheThao.TenSan : "")
                .ThenBy(b => b.GioBatDau)
                .ToListAsync();

            // Danh sách sân để lọc nhanh trên giao diện
            ViewBag.DanhSachSan = new SelectList(
                await _context.SanTheThaos.OrderBy(s => s.TenSan).AsNoTracking().ToListAsync(),
                "MaSan", "TenSan", maSan);

            return View(danhSachBangGia);
        }

        // =========================================================================================
        // GET: /BangGia/Create?maSan=...
        // Nghiệp vụ: Hiển thị form thêm mới bảng giá cho sân (Mục 6.3 file Word)
        // Phân quyền: Chỉ Admin hoặc NhanVien mới được quyền thêm bảng giá (Module 1 - Chiến)
        // =========================================================================================
        [PhanQuyen("Admin", "NhanVien")]
        public async Task<IActionResult> Create(int? maSan)
        {
            var vm = new BangGiaFormViewModel
            {
                MaSan = maSan ?? 0,
                NgayApDungTu = DateTime.Today,
                NgayApDungDen = DateTime.Today.AddYears(1),
                GioBatDau = new TimeSpan(6, 0, 0),
                GioKetThuc = new TimeSpan(17, 0, 0),
                DonGiaMoiGio = 100000,
                TrangThai = true,

                DanhSachSan = new SelectList(
                    await _context.SanTheThaos.Where(s => s.TinhTrang != "Ngừng sử dụng").OrderBy(s => s.TenSan).AsNoTracking().ToListAsync(),
                    "MaSan", "TenSan", maSan)
            };

            if (maSan.HasValue)
            {
                var san = await _context.SanTheThaos.FindAsync(maSan.Value);
                vm.TenSanHienThi = san?.TenSan;
            }

            return View(vm);
        }

        // =========================================================================================
        // POST: /BangGia/Create
        // Nghiệp vụ: Xác thực dữ liệu và kiểm tra ràng buộc khung giờ trước khi lưu (Mục 6.2, 13)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin", "NhanVien")]
        public async Task<IActionResult> Create(BangGiaFormViewModel vm)
        {
            // 1. Kiểm tra Giờ kết thúc phải lớn hơn Giờ bắt đầu (Mục 6.2 file Word)
            if (vm.GioKetThuc <= vm.GioBatDau)
            {
                ModelState.AddModelError(nameof(vm.GioKetThuc), "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
            }

            // 2. Kiểm tra Ngày áp dụng đến phải >= Ngày áp dụng từ
            if (vm.NgayApDungDen < vm.NgayApDungTu)
            {
                ModelState.AddModelError(nameof(vm.NgayApDungDen), "Ngày áp dụng đến phải lớn hơn hoặc bằng ngày áp dụng từ.");
            }

            // 3. Kiểm tra trùng lấn khung giờ đang hiệu lực của cùng một sân (Mục 6.2, 13 file Word)
            if (ModelState.IsValid && vm.TrangThai)
            {
                bool biChongLan = await _context.BangGias
                    .Where(b => b.MaSan == vm.MaSan && b.TrangThai)
                    .AnyAsync(b =>
                        // Giao nhau về khoảng ngày
                        b.NgayApDungTu <= vm.NgayApDungDen && b.NgayApDungDen >= vm.NgayApDungTu &&
                        // Giao nhau về khoảng giờ: BatDauMoi < KetThucCu && KetThucMoi > BatDauCu
                        vm.GioBatDau < b.GioKetThuc && vm.GioKetThuc > b.GioBatDau
                    );

                if (biChongLan)
                {
                    ModelState.AddModelError(string.Empty, "Khung giờ này đã bị trùng lặp hoặc chồng lấn với một bảng giá đang hiệu lực của sân.");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.DanhSachSan = new SelectList(
                    await _context.SanTheThaos.Where(s => s.TinhTrang != "Ngừng sử dụng").OrderBy(s => s.TenSan).AsNoTracking().ToListAsync(),
                    "MaSan", "TenSan", vm.MaSan);
                return View(vm);
            }

            var bangGia = new BangGia
            {
                MaSan = vm.MaSan,
                TenBangGia = vm.TenBangGia.Trim(),
                GioBatDau = vm.GioBatDau,
                GioKetThuc = vm.GioKetThuc,
                DonGiaMoiGio = vm.DonGiaMoiGio,
                NgayApDungTu = vm.NgayApDungTu,
                NgayApDungDen = vm.NgayApDungDen,
                TrangThai = vm.TrangThai
            };

            _context.BangGias.Add(bangGia);
            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã thêm bảng giá \"{bangGia.TenBangGia}\" thành công!";
            return RedirectToAction(nameof(Index), new { maSan = bangGia.MaSan });
        }

        // =========================================================================================
        // GET: /BangGia/Edit/{id}
        // Nghiệp vụ: Hiển thị form chỉnh sửa bảng giá (Mục 6.3 file Word)
        // =========================================================================================
        [PhanQuyen("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bg = await _context.BangGias.Include(b => b.SanTheThao).FirstOrDefaultAsync(b => b.MaBangGia == id);
            if (bg == null)
            {
                return NotFound();
            }

            var vm = new BangGiaFormViewModel
            {
                MaBangGia = bg.MaBangGia,
                MaSan = bg.MaSan,
                TenBangGia = bg.TenBangGia,
                GioBatDau = bg.GioBatDau,
                GioKetThuc = bg.GioKetThuc,
                DonGiaMoiGio = bg.DonGiaMoiGio,
                NgayApDungTu = bg.NgayApDungTu,
                NgayApDungDen = bg.NgayApDungDen,
                TrangThai = bg.TrangThai,
                TenSanHienThi = bg.SanTheThao?.TenSan,

                DanhSachSan = new SelectList(
                    await _context.SanTheThaos.OrderBy(s => s.TenSan).AsNoTracking().ToListAsync(),
                    "MaSan", "TenSan", bg.MaSan)
            };

            return View(vm);
        }

        // =========================================================================================
        // POST: /BangGia/Edit/{id}
        // Nghiệp vụ: Cập nhật thông tin bảng giá có kiểm tra ràng buộc khung giờ (Mục 6.2, 13)
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin", "NhanVien")]
        public async Task<IActionResult> Edit(int id, BangGiaFormViewModel vm)
        {
            if (id != vm.MaBangGia)
            {
                return NotFound();
            }

            // 1. Kiểm tra Giờ kết thúc > Giờ bắt đầu
            if (vm.GioKetThuc <= vm.GioBatDau)
            {
                ModelState.AddModelError(nameof(vm.GioKetThuc), "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
            }

            // 2. Kiểm tra Ngày áp dụng đến >= Ngày áp dụng từ
            if (vm.NgayApDungDen < vm.NgayApDungTu)
            {
                ModelState.AddModelError(nameof(vm.NgayApDungDen), "Ngày áp dụng đến phải lớn hơn hoặc bằng ngày áp dụng từ.");
            }

            // 3. Kiểm tra chồng lấn khung giờ với các bảng giá khác của cùng sân (trừ chính nó)
            if (ModelState.IsValid && vm.TrangThai)
            {
                bool biChongLan = await _context.BangGias
                    .Where(b => b.MaSan == vm.MaSan && b.TrangThai && b.MaBangGia != id)
                    .AnyAsync(b =>
                        b.NgayApDungTu <= vm.NgayApDungDen && b.NgayApDungDen >= vm.NgayApDungTu &&
                        vm.GioBatDau < b.GioKetThuc && vm.GioKetThuc > b.GioBatDau
                    );

                if (biChongLan)
                {
                    ModelState.AddModelError(string.Empty, "Khung giờ này bị trùng lặp hoặc chồng lấn với một bảng giá đang hiệu lực khác của sân.");
                }
            }

            if (!ModelState.IsValid)
            {
                vm.DanhSachSan = new SelectList(
                    await _context.SanTheThaos.OrderBy(s => s.TenSan).AsNoTracking().ToListAsync(),
                    "MaSan", "TenSan", vm.MaSan);
                return View(vm);
            }

            var bgCanSua = await _context.BangGias.FindAsync(id);
            if (bgCanSua == null)
            {
                return NotFound();
            }

            bgCanSua.MaSan = vm.MaSan;
            bgCanSua.TenBangGia = vm.TenBangGia.Trim();
            bgCanSua.GioBatDau = vm.GioBatDau;
            bgCanSua.GioKetThuc = vm.GioKetThuc;
            bgCanSua.DonGiaMoiGio = vm.DonGiaMoiGio;
            bgCanSua.NgayApDungTu = vm.NgayApDungTu;
            bgCanSua.NgayApDungDen = vm.NgayApDungDen;
            bgCanSua.TrangThai = vm.TrangThai;

            await _context.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã cập nhật bảng giá \"{bgCanSua.TenBangGia}\" thành công!";
            return RedirectToAction(nameof(Index), new { maSan = bgCanSua.MaSan });
        }
    }
}
