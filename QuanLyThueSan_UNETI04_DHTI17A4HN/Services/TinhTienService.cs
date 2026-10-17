// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Service tính tiền sân, tiền dịch vụ và tổng tiền của một đơn đặt sân
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public record KetQuaTinhTien(decimal TienSan, decimal TienDichVu, decimal TongTien);

public class TinhTienService
{
    private readonly ApplicationDbContext _db;

    public TinhTienService(ApplicationDbContext db)
    {
        _db = db;
    }

    // Tiền dịch vụ = Tổng(SoLuong × DonGia) của các dòng SuDungDichVu thuộc đơn.
    // Dùng DonGia đã lưu lúc sử dụng (không lấy giá hiện tại trong bảng DichVu). Không có dịch vụ → 0.
    public async Task<decimal> TinhTienDichVuAsync(int maDatSan)
    {
        return await _db.SuDungDichVus
            .AsNoTracking()
            .Where(x => x.MaDatSan == maDatSan)
            .SumAsync(x => x.SoLuong * x.DonGia);
    }

    // Tiền sân = số giờ sử dụng × đơn giá mỗi giờ của bảng giá đang hiệu lực.
    // Không cho nhập tay: luôn lấy từ BangGia. Không tìm được thì báo lỗi, không tự đoán giá.
    public async Task<decimal> TinhTienSanAsync(int maDatSan)
    {
        var don = await _db.DatSans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaDatSan == maDatSan);

        if (don is null)
            throw new InvalidOperationException("Không tìm thấy đơn đặt sân.");

        if (don.GioKetThuc <= don.GioBatDau)
            throw new InvalidOperationException("Giờ kết thúc phải lớn hơn giờ bắt đầu.");

        var ngay = don.NgaySuDung.Date;

        // Bảng giá phù hợp: đúng sân, đang hoạt động, ngày nằm trong khoảng áp dụng,
        // khung giờ chứa TRỌN khoảng giờ của đơn (đơn vắt qua 2 khung giá sẽ không khớp → báo lỗi).
        var bangGia = await _db.BangGias
            .AsNoTracking()
            .Where(b => b.MaSan == don.MaSan
                        && b.TrangThai
                        && b.NgayApDungTu <= ngay
                        && b.NgayApDungDen >= ngay
                        && b.GioBatDau <= don.GioBatDau
                        && b.GioKetThuc >= don.GioKetThuc)
            .OrderByDescending(b => b.NgayApDungTu)
            .FirstOrDefaultAsync();

        if (bangGia is null)
            throw new InvalidOperationException(
                "Không có bảng giá phù hợp cho sân, ngày và khung giờ của đơn đặt sân.");

        // Tính theo phút / 60 để giờ lẻ (VD 1,5 giờ) vẫn chính xác.
        decimal soGio = (decimal)(don.GioKetThuc - don.GioBatDau).TotalMinutes / 60m;
        return soGio * bangGia.DonGiaMoiGio;
    }

    // Tổng tiền = Tiền sân + Tiền dịch vụ
    public async Task<KetQuaTinhTien> TinhTatCaAsync(int maDatSan)
    {
        var tienSan = await TinhTienSanAsync(maDatSan);
        var tienDichVu = await TinhTienDichVuAsync(maDatSan);
        return new KetQuaTinhTien(tienSan, tienDichVu, tienSan + tienDichVu);
    }
}
