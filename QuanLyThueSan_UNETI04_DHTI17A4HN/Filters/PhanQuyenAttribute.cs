// Họ và tên: Khuất Anh Chiến
// Mã sinh viên: 23103100234
// Nội dung thực hiện: Module 1 - Bộ lọc phân quyền kiểm tra tại Controller (Mục 5.3 file Word)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class PhanQuyenAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] _cacVaiTroChoPhep;

        // Dùng: [PhanQuyen("Admin", "NhanVien")] - Chuỗi rỗng = mọi vai trò đã đăng nhập đều được
        public PhanQuyenAttribute(params string[] cacVaiTroChoPhep)
        {
            _cacVaiTroChoPhep = cacVaiTroChoPhep;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = context.HttpContext.Session;

            // Chưa đăng nhập -> chuyển về trang đăng nhập, giữ nguyên đường dẫn muốn truy cập
            if (session.GetInt32(Helpers.SessionKeys.MaTaiKhoan) is null)
            {
                var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
                context.Result = new RedirectResult($"/TaiKhoan/DangNhap?returnUrl={Uri.EscapeDataString(returnUrl)}");
                return;
            }

            // Đã đăng nhập nhưng sai vai trò -> trang từ chối truy cập
            var vaiTro = session.GetString(Helpers.SessionKeys.VaiTro);
            if (_cacVaiTroChoPhep.Length > 0 && !_cacVaiTroChoPhep.Contains(vaiTro))
            {
                context.Result = new RedirectResult("/TaiKhoan/TruyCapBiChan");
                return;
            }

            await next();
        }
    }
}
