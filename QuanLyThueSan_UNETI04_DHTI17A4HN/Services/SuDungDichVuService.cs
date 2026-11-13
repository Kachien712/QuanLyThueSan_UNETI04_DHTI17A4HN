// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 4 - Ghi nhận dịch vụ theo đơn, chốt đơn giá và cập nhật tồn kho.
using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public record KetQuaSuDungDichVu(bool ThanhCong, string ThongBao);

public class SuDungDichVuService(ApplicationDbContext db)
{
    public async Task<KetQuaSuDungDichVu> KetThucSanAsync(int maDatSan, CancellationToken ct = default)
    {
        // Cập nhật có điều kiện để hai nhân viên không thể cùng kết thúc một lượt sân.
        var soDong = await db.DatSans
            .Where(x => x.MaDatSan == maDatSan && x.TrangThai == "Đang sử dụng")
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.TrangThai, "Chờ thanh toán"), ct);

        return soDong == 1
            ? new(true, "Đã kết thúc lượt sân. Đơn đã được chuyển sang chờ thanh toán.")
            : new(false, "Đơn không còn ở trạng thái Đang sử dụng. Hãy tải lại danh sách.");
    }

    public async Task<KetQuaSuDungDichVu> ThemAsync(int maDatSan, int maDichVu, int soLuong, CancellationToken ct = default)
    {
        if (soLuong is < 1 or > 1000)
            return new(false, "Số lượng phải từ 1 đến 1000.");

        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        var donDangSuDung = await db.DatSans.AsNoTracking()
            .AnyAsync(x => x.MaDatSan == maDatSan && x.TrangThai == "Đang sử dụng", ct);
        if (!donDangSuDung)
            return new(false, "Chỉ được thêm dịch vụ cho đơn đang sử dụng sân.");

        var dichVu = await db.DichVus.AsNoTracking()
            .SingleOrDefaultAsync(x => x.MaDichVu == maDichVu, ct);
        if (dichVu is null || !dichVu.TrangThai)
            return new(false, "Dịch vụ không tồn tại hoặc đã ngừng hoạt động.");
        if (dichVu.DonGia < 0)
            return new(false, "Đơn giá dịch vụ không hợp lệ.");

        // Null nghĩa là dịch vụ không quản lý tồn kho. UPDATE có điều kiện tránh bán quá số tồn khi nhiều nhân viên thao tác.
        if (dichVu.SoLuongTon.HasValue)
        {
            var soDong = await db.DichVus
                .Where(x => x.MaDichVu == maDichVu && x.TrangThai &&
                    x.SoLuongTon.HasValue && x.SoLuongTon >= soLuong)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.SoLuongTon, x => x.SoLuongTon - soLuong), ct);
            if (soDong != 1)
                return new(false, "Số lượng tồn không đủ hoặc dịch vụ đã thay đổi. Hãy tải lại trang.");
        }

        db.SuDungDichVus.Add(new SuDungDichVu
        {
            MaDatSan = maDatSan,
            MaDichVu = maDichVu,
            SoLuong = soLuong,
            DonGia = dichVu.DonGia,
            ThanhTien = soLuong * dichVu.DonGia,
            ThoiGianThem = DateTime.Now
        });
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return new(true, "Đã ghi nhận dịch vụ sử dụng.");
    }

    public async Task<KetQuaSuDungDichVu> XoaAsync(int maDatSan, int maSuDung, CancellationToken ct = default)
    {
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
        var donDangSuDung = await db.DatSans.AsNoTracking()
            .AnyAsync(x => x.MaDatSan == maDatSan && x.TrangThai == "Đang sử dụng", ct);
        if (!donDangSuDung)
            return new(false, "Chỉ được sửa dịch vụ của đơn đang sử dụng sân.");

        var phatSinh = await db.SuDungDichVus.AsNoTracking()
            .SingleOrDefaultAsync(x => x.MaSuDung == maSuDung && x.MaDatSan == maDatSan, ct);
        if (phatSinh is null)
            return new(false, "Không tìm thấy dòng dịch vụ trong đơn.");

        // Xóa có điều kiện trước khi hoàn kho, tránh hai yêu cầu cùng hoàn tồn cho một dòng.
        var soDong = await db.SuDungDichVus
            .Where(x => x.MaSuDung == maSuDung && x.MaDatSan == maDatSan)
            .ExecuteDeleteAsync(ct);
        if (soDong != 1)
            return new(false, "Dòng dịch vụ đã được thay đổi. Hãy tải lại trang.");

        var soDongHoanKho = await db.DichVus
            .Where(x => x.MaDichVu == phatSinh.MaDichVu && x.SoLuongTon.HasValue &&
                x.SoLuongTon <= int.MaxValue - phatSinh.SoLuong)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.SoLuongTon, x => x.SoLuongTon + phatSinh.SoLuong), ct);
        if (soDongHoanKho == 0)
        {
            var dichVu = await db.DichVus.AsNoTracking()
                .SingleOrDefaultAsync(x => x.MaDichVu == phatSinh.MaDichVu, ct);
            if (dichVu is null || dichVu.SoLuongTon.HasValue)
                return new(false, "Không thể hoàn lại tồn kho; thao tác xóa đã được hủy.");
        }

        await tx.CommitAsync(ct);
        return new(true, "Đã xóa dòng dịch vụ và hoàn lại tồn kho nếu có quản lý.");
    }
}
