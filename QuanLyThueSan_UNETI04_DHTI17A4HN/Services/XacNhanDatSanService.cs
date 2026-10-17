// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 1 - kiểm tra và xác nhận/từ chối đơn đặt sân.
using System.Data;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public record KetQuaXacNhan(bool ThanhCong, string ThongBao);

public class XacNhanDatSanService(ApplicationDbContext db)
{
    public async Task<KetQuaXacNhan> XacNhanAsync(int maDatSan, CancellationToken ct = default)
    {
        // SERIALIZABLE giữ kết quả kiểm tra lịch ổn định cho đến khi lưu trạng thái.
        await using var tx = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var don = await db.DatSans.Include(x => x.SanTheThao)
            .SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, ct);
        if (don is null) return new(false, "Không tìm thấy đơn đặt sân.");
        if (don.TrangThai != "Chờ xác nhận") return new(false, "Đơn không còn ở trạng thái Chờ xác nhận.");
        if (don.SanTheThao?.TinhTrang != "Đang hoạt động")
            return new(false, "Sân không còn hoạt động.");
        if (don.GioBatDau < TimeSpan.Zero || don.GioKetThuc > TimeSpan.FromDays(1) ||
            don.GioBatDau >= don.GioKetThuc ||
            don.NgaySuDung.Date.Add(don.GioBatDau) <= DateTime.Now)
            return new(false, "Thời gian sử dụng không hợp lệ hoặc đã qua.");

        var trungLich = await db.DatSans.AnyAsync(x =>
            x.MaDatSan != maDatSan && x.MaSan == don.MaSan &&
            x.NgaySuDung.Date == don.NgaySuDung.Date &&
            (x.TrangThai == "Đã xác nhận" || x.TrangThai == "Đang sử dụng") &&
            don.GioBatDau < x.GioKetThuc && don.GioKetThuc > x.GioBatDau, ct);
        if (trungLich) return new(false, "Sân đã có đơn hiệu lực trùng giờ.");

        // Quy tắc tuần 1: một bảng giá hiệu lực phải phủ toàn bộ lượt đặt.
        var coGia = await db.BangGias.AnyAsync(x =>
            x.MaSan == don.MaSan && x.TrangThai && x.DonGiaMoiGio > 0 &&
            x.NgayApDungTu.Date <= don.NgaySuDung.Date &&
            x.NgayApDungDen.Date >= don.NgaySuDung.Date &&
            x.GioBatDau <= don.GioBatDau && x.GioKetThuc >= don.GioKetThuc, ct);
        if (!coGia) return new(false, "Không có bảng giá đang áp dụng phủ toàn bộ khung giờ.");

        don.TrangThai = "Đã xác nhận";
        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
        return new(true, "Đã xác nhận đơn đặt sân.");
    }

    public async Task<KetQuaXacNhan> TuChoiAsync(int maDatSan, string? lyDo, CancellationToken ct = default)
    {
        lyDo = lyDo?.Trim();
        if (string.IsNullOrWhiteSpace(lyDo) || lyDo.Length > 450)
            return new(false, "Lý do từ chối phải có từ 1 đến 450 ký tự.");

        var don = await db.DatSans.SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, ct);
        if (don is null) return new(false, "Không tìm thấy đơn đặt sân.");
        if (don.TrangThai != "Chờ xác nhận")
            return new(false, "Chỉ được từ chối đơn đang Chờ xác nhận.");

        var ghiChuMoi = string.IsNullOrWhiteSpace(don.GhiChu)
            ? $"Từ chối: {lyDo}"
            : $"{don.GhiChu.Trim()}\nTừ chối: {lyDo}";
        if (ghiChuMoi.Length > 500)
            return new(false, "Lý do quá dài so với ghi chú hiện có của đơn.");

        don.TrangThai = "Đã hủy";
        don.GhiChu = ghiChuMoi;
        await db.SaveChangesAsync(ct);
        return new(true, "Đã từ chối đơn và lưu lý do.");
    }
}
