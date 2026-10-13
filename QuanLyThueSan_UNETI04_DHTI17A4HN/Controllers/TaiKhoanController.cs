// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Đăng nhập, đăng xuất, đăng ký tài khoản và kiểm tra phân quyền (Mục 5.1 - 5.3 file Word)

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
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
