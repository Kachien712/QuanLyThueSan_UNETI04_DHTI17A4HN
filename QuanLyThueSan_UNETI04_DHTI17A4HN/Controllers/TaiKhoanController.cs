// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Đăng nhập, đăng xuất, đăng ký, phân quyền và quản trị tài khoản Admin (Mục 5.1 - 5.3 file Word)

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==================== ĐĂNG NHẬP ====================
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan) is not null)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new DangNhapViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var tenDangNhap = model.TenDangNhap.Trim();
            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap);

            if (taiKhoan == null || !MatKhauHelper.KiemTra(taiKhoan.MatKhau, model.MatKhau))
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản này đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            await LuuDangNhapVaoSession(taiKhoan);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Admin đăng nhập xong vào thẳng Dashboard thay vì trang chủ công khai
            if (taiKhoan.VaiTro == "Admin")
                return RedirectToAction("Index", "Dashboard");

            return RedirectToAction("Index", "Home");
        }

        // ==================== ĐĂNG KÝ ====================
        [HttpGet]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan) is not null)
                return RedirectToAction("Index", "Home");

            return View(new DangKyViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tenDangNhap = model.TenDangNhap.Trim();
            var email = model.Email.Trim();
            var soDienThoai = model.SoDienThoai.Trim();

            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap))
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");

            if (await _context.TaiKhoans.AnyAsync(t => t.Email == email)
                || await _context.KhachHangs.AnyAsync(k => k.Email == email))
                ModelState.AddModelError("Email", "Email này đã được sử dụng.");

            if (await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == soDienThoai))
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng.");

            if (!ModelState.IsValid)
                return View(model);

            // Tạo tài khoản rồi tạo khách hàng liên kết (TaiKhoan 1 - 0..1 KhachHang)
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                MatKhau = MatKhauHelper.MaHoa(model.MatKhau),
                HoTen = model.HoTen.Trim(),
                Email = email,
                VaiTro = "KhachHang",
                TrangThai = true
            };
            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            var khachHang = new KhachHang
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = taiKhoan.HoTen,
                SoDienThoai = soDienThoai,
                Email = email,
                TrangThai = true
            };
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();

            await LuuDangNhapVaoSession(taiKhoan);

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
            return RedirectToAction("Index", "Home");
        }

        // ==================== ĐĂNG XUẤT ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangXuat()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // ==================== TRANG TỪ CHỐI TRUY CẬP ====================
        [HttpGet]
        public IActionResult TruyCapBiChan()
        {
            return View();
        }

        // =========================================================================================
        // GET: /TaiKhoan/Index
        // Nghiệp vụ: Admin quản lý danh sách tài khoản, tìm kiếm, lọc vai trò/trạng thái và phân trang (Mục 1, 17.2)
        // =========================================================================================
        [HttpGet]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> Index(string? tuKhoa, string? vaiTroFilter, bool? trangThaiFilter, int pageIndex = 1, int pageSize = 10)
        {
            var query = _context.TaiKhoans
                .Include(t => t.KhachHang)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(t => t.TenDangNhap.ToLower().Contains(keyword)
                                      || t.HoTen.ToLower().Contains(keyword)
                                      || t.Email.ToLower().Contains(keyword));
            }

            if (!string.IsNullOrWhiteSpace(vaiTroFilter))
            {
                query = query.Where(t => t.VaiTro == vaiTroFilter);
            }

            if (trangThaiFilter.HasValue)
            {
                query = query.Where(t => t.TrangThai == trangThaiFilter.Value);
            }

            query = query.OrderByDescending(t => t.MaTaiKhoan);

            var tongSo = await _context.TaiKhoans.CountAsync();
            var hoatDong = await _context.TaiKhoans.CountAsync(t => t.TrangThai);
            var biKhoa = tongSo - hoatDong;
            var soAdmin = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "Admin");
            var soNhanVien = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "NhanVien");
            var soKhachHang = await _context.TaiKhoans.CountAsync(t => t.VaiTro == "KhachHang");

            var pagedData = await PaginatedList<TaiKhoan>.CreateAsync(query, pageIndex, pageSize);

            var danhSachVaiTro = new List<SelectListItem>
            {
                new() { Value = "", Text = "Tất cả vai trò", Selected = string.IsNullOrEmpty(vaiTroFilter) },
                new() { Value = "Admin", Text = "Admin (Quản trị viên)", Selected = vaiTroFilter == "Admin" },
                new() { Value = "NhanVien", Text = "Nhân viên trực sân", Selected = vaiTroFilter == "NhanVien" },
                new() { Value = "KhachHang", Text = "Khách hàng", Selected = vaiTroFilter == "KhachHang" }
            };

            var danhSachTrangThai = new List<SelectListItem>
            {
                new() { Value = "", Text = "Tất cả trạng thái", Selected = !trangThaiFilter.HasValue },
                new() { Value = "true", Text = "Đang hoạt động", Selected = trangThaiFilter == true },
                new() { Value = "false", Text = "Đang bị khóa", Selected = trangThaiFilter == false }
            };

            var vm = new TaiKhoanIndexViewModel
            {
                DanhSachTaiKhoan = pagedData,
                TuKhoa = tuKhoa,
                VaiTroFilter = vaiTroFilter,
                TrangThaiFilter = trangThaiFilter,
                TongSoTaiKhoan = tongSo,
                SoTaiKhoanHoatDong = hoatDong,
                SoTaiKhoanBiKhoa = biKhoa,
                SoAdmin = soAdmin,
                SoNhanVien = soNhanVien,
                SoKhachHang = soKhachHang,
                DanhSachVaiTro = new SelectList(danhSachVaiTro, "Value", "Text", vaiTroFilter),
                DanhSachTrangThai = new SelectList(danhSachTrangThai, "Value", "Text", trangThaiFilter?.ToString().ToLower())
            };

            return View(vm);
        }

        // =========================================================================================
        // GET: /TaiKhoan/KiemTraDangKy?truong=TenDangNhap&giaTri=abc
        // Nghiệp vụ: AJAX kiểm tra trùng dữ liệu ngay khi khách hàng nhập ở form Đăng ký.
        // Controller vẫn là nơi quyết định cuối cùng; POST DangKy vẫn kiểm tra lại (không tin client).
        // =========================================================================================
        [HttpGet]
        public async Task<IActionResult> KiemTraDangKy(string? truong, string? giaTri)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
                return Json(new { trung = false, thongBao = "" });

            var gt = giaTri.Trim();
            bool trung;
            string thongBao;

            switch (truong)
            {
                case "TenDangNhap":
                    trung = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == gt.ToLower());
                    thongBao = "Tên đăng nhập này đã có người sử dụng.";
                    break;
                case "Email":
                    trung = await _context.TaiKhoans.AnyAsync(t => t.Email.ToLower() == gt.ToLower())
                         || await _context.KhachHangs.AnyAsync(k => k.Email.ToLower() == gt.ToLower());
                    thongBao = "Email này đã được sử dụng.";
                    break;
                case "SoDienThoai":
                    trung = await _context.KhachHangs.AnyAsync(k => k.SoDienThoai == gt);
                    thongBao = "Số điện thoại này đã được sử dụng.";
                    break;
                default:
                    return BadRequest();
            }

            return Json(new { trung, thongBao = trung ? thongBao : "Có thể sử dụng." });
        }

        // =========================================================================================
        // GET: /TaiKhoan/KiemTraTaiKhoan?truong=TenDangNhap&giaTri=abc
        // Nghiệp vụ: AJAX kiểm tra trùng tên đăng nhập / email ở form Admin tạo tài khoản.
        // =========================================================================================
        [HttpGet]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> KiemTraTaiKhoan(string? truong, string? giaTri)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
                return Json(new { trung = false, thongBao = "" });

            var gt = giaTri.Trim();
            bool trung;
            string thongBao;

            switch (truong)
            {
                case "TenDangNhap":
                    trung = await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == gt.ToLower());
                    thongBao = "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.";
                    break;
                case "Email":
                    trung = await _context.TaiKhoans.AnyAsync(t => t.Email.ToLower() == gt.ToLower());
                    thongBao = "Địa chỉ Email này đã được sử dụng.";
                    break;
                default:
                    return BadRequest();
            }

            return Json(new { trung, thongBao = trung ? thongBao : "Có thể sử dụng." });
        }

        // =========================================================================================
        // GET: /TaiKhoan/Create
        // =========================================================================================
        [HttpGet]
        [PhanQuyen("Admin")]
        public IActionResult Create()
        {
            return View(new TaiKhoanCreateViewModel());
        }

        // =========================================================================================
        // POST: /TaiKhoan/Create
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> Create(TaiKhoanCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var tenDangNhap = model.TenDangNhap.Trim();
            var email = model.Email.Trim();

            if (await _context.TaiKhoans.AnyAsync(t => t.TenDangNhap.ToLower() == tenDangNhap.ToLower()))
            {
                ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác.");
            }

            if (await _context.TaiKhoans.AnyAsync(t => t.Email.ToLower() == email.ToLower()))
            {
                ModelState.AddModelError(nameof(model.Email), "Địa chỉ Email này đã được sử dụng.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                MatKhau = MatKhauHelper.MaHoa(model.MatKhau),
                HoTen = model.HoTen.Trim(),
                Email = email,
                VaiTro = model.VaiTro,
                TrangThai = model.TrangThai
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // Nếu tạo tài khoản vai trò Khách hàng và có cung cấp số điện thoại -> tạo bản ghi KhachHang tương ứng
            if (model.VaiTro == "KhachHang")
            {
                var soDienThoai = string.IsNullOrWhiteSpace(model.SoDienThoai)
                    ? "09" + Random.Shared.Next(10000000, 99999999)
                    : model.SoDienThoai.Trim();

                var khachHang = new KhachHang
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = taiKhoan.HoTen,
                    SoDienThoai = soDienThoai,
                    Email = email,
                    TrangThai = model.TrangThai
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = $"Đã khởi tạo tài khoản \"{taiKhoan.TenDangNhap}\" ({taiKhoan.VaiTro}) thành công!";
            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // POST: /TaiKhoan/DoiTrangThai/{id}
        // Nghiệp vụ: Admin Khóa hoặc Mở khóa tài khoản (Mục 5.1: tài khoản bị khóa không được đăng nhập)
        // Dùng chung logic với phiên bản AJAX bên dưới.
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var ketQua = await XuLyDoiTrangThai(id);

            if (!ketQua.TimThay)
                return NotFound();

            if (ketQua.ThanhCong)
                TempData["SuccessMessage"] = ketQua.ThongBao;
            else
                TempData["ErrorMessage"] = ketQua.ThongBao;

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // POST: /TaiKhoan/DoiTrangThaiAjax/{id}
        // Nghiệp vụ: AJAX khóa / mở khóa tài khoản, trả JSON để cập nhật bảng mà không tải lại trang.
        // Vẫn giữ [ValidateAntiForgeryToken] và [PhanQuyen("Admin")] nên không thể gọi lậu từ client.
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> DoiTrangThaiAjax(int id)
        {
            var ketQua = await XuLyDoiTrangThai(id);

            if (!ketQua.TimThay)
                return NotFound();

            return Json(new
            {
                thanhCong = ketQua.ThanhCong,
                thongBao = ketQua.ThongBao,
                trangThai = ketQua.TrangThaiMoi
            });
        }

        // Hàm dùng chung: đảo trạng thái tài khoản, chặn tự khóa chính mình và đồng bộ KhachHang
        private async Task<(bool TimThay, bool ThanhCong, string ThongBao, bool TrangThaiMoi)> XuLyDoiTrangThai(int id)
        {
            var hienTaiId = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            if (hienTaiId == id)
            {
                return (true, false, "Bạn không thể tự khóa tài khoản Admin đang đăng nhập của chính mình!", true);
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.KhachHang)
                .FirstOrDefaultAsync(t => t.MaTaiKhoan == id);

            if (taiKhoan == null)
                return (false, false, "Không tìm thấy tài khoản.", false);

            taiKhoan.TrangThai = !taiKhoan.TrangThai;

            // Đồng bộ trạng thái với khách hàng nếu có
            if (taiKhoan.KhachHang != null)
            {
                taiKhoan.KhachHang.TrangThai = taiKhoan.TrangThai;
            }

            await _context.SaveChangesAsync();

            var trangThaiText = taiKhoan.TrangThai ? "Đang hoạt động" : "Bị khóa";
            var thongBao = $"Tài khoản \"{taiKhoan.TenDangNhap}\" đã được chuyển sang trạng thái: {trangThaiText}.";
            return (true, true, thongBao, taiKhoan.TrangThai);
        }

        // =========================================================================================
        // POST: /TaiKhoan/DatLaiMatKhau/{id}
        // Nghiệp vụ: Admin đặt lại mật khẩu về mặc định (123456) cho tài khoản
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> DatLaiMatKhau(int id, string? matKhauMoi)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            var pass = string.IsNullOrWhiteSpace(matKhauMoi) ? "123456" : matKhauMoi.Trim();
            taiKhoan.MatKhau = MatKhauHelper.MaHoa(pass);

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu cho tài khoản \"{taiKhoan.TenDangNhap}\" thành công (Mật khẩu mới: {pass})!";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================================================
        // POST: /TaiKhoan/DoiVaiTro/{id}
        // =========================================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [PhanQuyen("Admin")]
        public async Task<IActionResult> DoiVaiTro(int id, string vaiTroMoi)
        {
            var hienTaiId = HttpContext.Session.GetInt32(SessionKeys.MaTaiKhoan);
            if (hienTaiId == id && vaiTroMoi != "Admin")
            {
                TempData["ErrorMessage"] = "Bạn không thể tự hạ quyền Admin của chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            if (vaiTroMoi is not ("Admin" or "NhanVien" or "KhachHang"))
            {
                TempData["ErrorMessage"] = "Vai trò không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            taiKhoan.VaiTro = vaiTroMoi;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã cập nhật vai trò cho tài khoản \"{taiKhoan.TenDangNhap}\" thành \"{vaiTroMoi}\"!";
            return RedirectToAction(nameof(Index));
        }

        // Ghi thông tin vào Session (bắt buộc theo đề tài) + đăng nhập Cookie cho [Authorize]/Forbid()
        private async Task LuuDangNhapVaoSession(TaiKhoan taiKhoan)
        {
            HttpContext.Session.SetInt32(SessionKeys.MaTaiKhoan, taiKhoan.MaTaiKhoan);
            HttpContext.Session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);
            HttpContext.Session.SetString(SessionKeys.VaiTro, taiKhoan.VaiTro);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, taiKhoan.MaTaiKhoan.ToString()),
                new(ClaimTypes.Name, taiKhoan.HoTen),
                new(ClaimTypes.Role, taiKhoan.VaiTro)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}
