// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Bộ lọc phân quyền kiểm tra tại Controller (Mục 5.3 file Word)
// - Kiểm tra đăng nhập và vai trò trên từng Request
// - Đọc lại TaiKhoan từ DB để khóa tài khoản/đổi vai trò có hiệu lực ngay lập tức

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PhanQuyenAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _cacVaiTroChoPhep;

        // Dùng: [PhanQuyen("Admin", "NhanVien")] - Không truyền tham số = mọi vai trò đã đăng nhập đều được
        public PhanQuyenAttribute(params string[] cacVaiTroChoPhep)
        {
            _cacVaiTroChoPhep = cacVaiTroChoPhep;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = context.HttpContext.Session;
            var maTaiKhoan = session.GetInt32(SessionKeys.MaTaiKhoan);

            // Chưa đăng nhập -> chuyển về trang đăng nhập, giữ nguyên đường dẫn muốn truy cập
            if (maTaiKhoan is null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectResult($"/TaiKhoan/DangNhap?returnUrl={Uri.EscapeDataString(returnUrl)}");
                return;
            }

            // Đọc lại tài khoản từ DB: không tin tuyệt đối vào VaiTro/TrangThai đang lưu trong Session
            var db = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
            var taiKhoan = await db.TaiKhoans
                .AsNoTracking()
                .Where(t => t.MaTaiKhoan == maTaiKhoan.Value)
                .Select(t => new { t.VaiTro, t.HoTen, t.TrangThai })
                .FirstOrDefaultAsync();

            // Tài khoản đã bị xóa hoặc bị khóa -> buộc đăng xuất ngay trong request hiện tại
            if (taiKhoan is null || !taiKhoan.TrangThai)
            {
                session.Clear();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Result = new RedirectResult("/TaiKhoan/DangNhap?biKhoa=1");
                return;
            }

            // Đồng bộ vai trò / họ tên mới nhất từ DB vào Session để các View hiển thị đúng
            if (session.GetString(SessionKeys.VaiTro) != taiKhoan.VaiTro)
                session.SetString(SessionKeys.VaiTro, taiKhoan.VaiTro);

            if (session.GetString(SessionKeys.HoTen) != taiKhoan.HoTen)
                session.SetString(SessionKeys.HoTen, taiKhoan.HoTen);

            // Đã đăng nhập nhưng sai vai trò -> trang từ chối truy cập
            if (_cacVaiTroChoPhep.Length > 0 && !_cacVaiTroChoPhep.Contains(taiKhoan.VaiTro))
            {
                context.Result = new RedirectResult("/TaiKhoan/TruyCapBiChan");
                return;
            }

            await next();
        }
    }
}
