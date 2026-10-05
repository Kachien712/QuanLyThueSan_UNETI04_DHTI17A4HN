/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Kiểm tra quyền Admin/Nhân viên tại Controller cho các nghiệp vụ vận hành sân.
 */
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class YeuCauVaiTroAttribute(params string[] vaiTros) : Attribute, IAsyncActionFilter
{
    private readonly string[] _vaiTros = vaiTros;

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var vaiTro = context.HttpContext.Session.GetString("VaiTro");
        if (string.IsNullOrWhiteSpace(vaiTro) ||
            !_vaiTros.Contains(vaiTro, StringComparer.OrdinalIgnoreCase))
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        await next();
    }
}
