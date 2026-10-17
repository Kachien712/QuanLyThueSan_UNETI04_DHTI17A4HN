// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 2 - nhận sân cho đơn đã xác nhận.
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public class NhanSanService(ApplicationDbContext db)
{
    public async Task<KetQuaXacNhan> NhanSanAsync(int maDatSan, CancellationToken ct = default)
    {
        var don = await db.DatSans.SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, ct);
        if (don is null) return new(false, "Không tìm thấy đơn đặt sân.");
        if (don.TrangThai != "Đã xác nhận" || don.ThoiDiemNhanSanThucTe.HasValue)
            return new(false, "Chỉ đơn Đã xác nhận và chưa nhận sân mới được nhận sân.");

        // Chặn nhận nhầm ngày; việc xử lý đến muộn/sớm trong ngày do nhân viên quyết định.
        if (don.NgaySuDung.Date != DateTime.Today)
            return new(false, "Chỉ được nhận sân vào đúng ngày sử dụng.");

        var thoiDiemNhan = DateTime.Now;
        if (thoiDiemNhan.TimeOfDay >= don.GioKetThuc)
            return new(false, "Đã qua giờ kết thúc đặt sân.");

        // Điều kiện trong UPDATE bảo đảm hai nhân viên không nhận cùng một đơn hai lần.
        var soDong = await db.DatSans
            .Where(x => x.MaDatSan == maDatSan &&
                x.TrangThai == "Đã xác nhận" && x.ThoiDiemNhanSanThucTe == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.TrangThai, "Đang sử dụng")
                .SetProperty(x => x.ThoiDiemNhanSanThucTe, thoiDiemNhan), ct);
        if (soDong != 1)
            return new(false, "Đơn đã được thay đổi bởi người khác. Hãy tải lại danh sách.");
        return new(true, "Nhận sân thành công; đơn chuyển sang Đang sử dụng.");
    }
}
