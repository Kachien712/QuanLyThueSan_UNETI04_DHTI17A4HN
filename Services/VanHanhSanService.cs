/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Xử lý nghiệp vụ Module 4: kiểm tra lại trùng lịch, xác nhận đơn,
 * nhận/trả sân, chốt dịch vụ sử dụng và cập nhật trạng thái đơn.
 */
using System.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Data;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models.Enums;
using QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Services;

public class VanHanhSanService(QuanLySanContext context) : IVanHanhSanService
{
    private readonly QuanLySanContext _context = context;

    public async Task<IReadOnlyList<VanHanhDatSanViewModel>> GetDanhSachAsync(
        TrangThaiDatSan? trangThai,
        CancellationToken cancellationToken = default)
    {
        IQueryable<DatSan> query = _context.DatSans
            .AsNoTracking()
            .Include(x => x.San)
            .Include(x => x.SuDungDichVus)
            .ThenInclude(x => x.DichVu);

        if (trangThai.HasValue)
        {
            query = query.Where(x => x.TrangThai == trangThai.Value);
        }

        var datSans = await query
            .OrderBy(x => x.NgaySuDung)
            .ThenBy(x => x.GioBatDau)
            .ToListAsync(cancellationToken);

        return datSans.Select(ToViewModel).ToList();
    }

    public async Task<VanHanhDatSanViewModel?> GetChiTietAsync(int maDatSan, CancellationToken cancellationToken = default)
    {
        var datSan = await _context.DatSans
            .AsNoTracking()
            .Include(x => x.San)
            .Include(x => x.SuDungDichVus)
            .ThenInclude(x => x.DichVu)
            .SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, cancellationToken);

        return datSan is null ? null : ToViewModel(datSan);
    }

    public async Task<IReadOnlyList<SelectListItem>> GetDichVuKhaDungAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DichVus
            .AsNoTracking()
            .Where(x => x.TrangThai && (!x.SoLuongTon.HasValue || x.SoLuongTon > 0))
            .OrderBy(x => x.TenDichVu)
            .Select(x => new SelectListItem
            {
                Value = x.MaDichVu.ToString(),
                Text = $"{x.TenDichVu} - {x.DonGia:N0} đ/{x.DonViTinh}"
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<OperationResult> XacNhanAsync(int maDatSan, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        var datSan = await _context.DatSans
            .Include(x => x.San)
            .SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, cancellationToken);

        if (datSan is null)
        {
            return OperationResult.Failure("Không tìm thấy đơn đặt sân.");
        }

        if (datSan.TrangThai != TrangThaiDatSan.ChoXacNhan)
        {
            return OperationResult.Failure("Chỉ xác nhận đơn đang ở trạng thái Chờ xác nhận.");
        }

        if (datSan.San is null || datSan.San.TinhTrang != TinhTrangSan.DangHoatDong)
        {
            return OperationResult.Failure("Sân không còn hoạt động nên không thể xác nhận đơn.");
        }

        if (datSan.GioKetThuc <= datSan.GioBatDau)
        {
            return OperationResult.Failure("Khoảng giờ sử dụng không hợp lệ.");
        }

        if (datSan.DonGiaSanDuKien <= 0)
        {
            return OperationResult.Failure("Không tìm thấy bảng giá hợp lệ cho đơn đặt sân.");
        }

        var coTrungLich = await _context.DatSans.AnyAsync(x =>
            x.MaDatSan != datSan.MaDatSan &&
            x.MaSan == datSan.MaSan &&
            x.NgaySuDung == datSan.NgaySuDung &&
            (x.TrangThai == TrangThaiDatSan.DaXacNhan || x.TrangThai == TrangThaiDatSan.DangSuDung) &&
            datSan.GioBatDau < x.GioKetThuc && datSan.GioKetThuc > x.GioBatDau,
            cancellationToken);

        if (coTrungLich)
        {
            return OperationResult.Failure("Sân đã có đơn được xác nhận hoặc đang sử dụng trùng khoảng thời gian này.");
        }

        datSan.TrangThai = TrangThaiDatSan.DaXacNhan;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return OperationResult.Success("Đã xác nhận đơn đặt sân.");
    }

    public async Task<OperationResult> TuChoiAsync(int maDatSan, string lyDo, CancellationToken cancellationToken = default)
    {
        var datSan = await _context.DatSans.SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, cancellationToken);

        if (datSan is null)
        {
            return OperationResult.Failure("Không tìm thấy đơn đặt sân.");
        }

        if (datSan.TrangThai != TrangThaiDatSan.ChoXacNhan)
        {
            return OperationResult.Failure("Chỉ từ chối đơn đang ở trạng thái Chờ xác nhận.");
        }

        if (string.IsNullOrWhiteSpace(lyDo))
        {
            return OperationResult.Failure("Phải ghi rõ lý do từ chối đơn.");
        }

        datSan.TrangThai = TrangThaiDatSan.DaHuy;
        datSan.LyDoTuChoi = lyDo.Trim();
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success("Đã từ chối đơn đặt sân và lưu lý do.");
    }

    public async Task<OperationResult> NhanSanAsync(int maDatSan, CancellationToken cancellationToken = default)
    {
        var datSan = await _context.DatSans.SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, cancellationToken);

        if (datSan is null)
        {
            return OperationResult.Failure("Không tìm thấy đơn đặt sân.");
        }

        if (datSan.TrangThai != TrangThaiDatSan.DaXacNhan)
        {
            return OperationResult.Failure("Chỉ đơn Đã xác nhận mới được nhận sân.");
        }

        datSan.TrangThai = TrangThaiDatSan.DangSuDung;
        datSan.ThoiDiemNhanThucTe = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success("Đã nhận sân, đơn chuyển sang trạng thái Đang sử dụng.");
    }

    public async Task<OperationResult> ThemDichVuAsync(
        int maDatSan,
        int maDichVu,
        int soLuong,
        CancellationToken cancellationToken = default)
    {
        if (soLuong <= 0)
        {
            return OperationResult.Failure("Số lượng dịch vụ phải lớn hơn 0.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);
        var datSan = await _context.DatSans.SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, cancellationToken);
        if (datSan is null)
        {
            return OperationResult.Failure("Không tìm thấy đơn đặt sân.");
        }

        if (datSan.TrangThai != TrangThaiDatSan.DangSuDung)
        {
            return OperationResult.Failure("Chỉ được thêm dịch vụ khi đơn đang sử dụng sân.");
        }

        var dichVu = await _context.DichVus.SingleOrDefaultAsync(x => x.MaDichVu == maDichVu, cancellationToken);
        if (dichVu is null || !dichVu.TrangThai)
        {
            return OperationResult.Failure("Dịch vụ không tồn tại hoặc đã ngừng hoạt động.");
        }

        if (dichVu.SoLuongTon.HasValue && dichVu.SoLuongTon.Value < soLuong)
        {
            return OperationResult.Failure("Số lượng tồn không đủ để thêm dịch vụ.");
        }

        var suDungDichVu = new SuDungDichVu
        {
            MaDatSan = datSan.MaDatSan,
            MaDichVu = dichVu.MaDichVu,
            SoLuong = soLuong,
            DonGia = dichVu.DonGia,
            ThanhTien = soLuong * dichVu.DonGia,
            ThoiGianThem = DateTime.Now
        };

        if (dichVu.SoLuongTon.HasValue)
        {
            dichVu.SoLuongTon -= soLuong;
        }

        _context.SuDungDichVus.Add(suDungDichVu);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return OperationResult.Success("Đã thêm dịch vụ và chốt đơn giá tại thời điểm sử dụng.");
    }

    public async Task<OperationResult> TraSanAsync(int maDatSan, CancellationToken cancellationToken = default)
    {
        var datSan = await _context.DatSans.SingleOrDefaultAsync(x => x.MaDatSan == maDatSan, cancellationToken);
        if (datSan is null)
        {
            return OperationResult.Failure("Không tìm thấy đơn đặt sân.");
        }

        if (datSan.TrangThai != TrangThaiDatSan.DangSuDung)
        {
            return OperationResult.Failure("Chỉ đơn Đang sử dụng mới được trả sân.");
        }

        datSan.TrangThai = TrangThaiDatSan.ChoThanhToan;
        datSan.ThoiDiemTraThucTe = DateTime.Now;
        await _context.SaveChangesAsync(cancellationToken);
        return OperationResult.Success("Đã trả sân, đơn chuyển sang trạng thái Chờ thanh toán.");
    }

    private static VanHanhDatSanViewModel ToViewModel(DatSan datSan) => new()
    {
        MaDatSan = datSan.MaDatSan,
        MaKhachHang = datSan.MaKhachHang,
        TenSan = datSan.San?.TenSan ?? $"Sân #{datSan.MaSan}",
        NgaySuDung = datSan.NgaySuDung,
        GioBatDau = datSan.GioBatDau,
        GioKetThuc = datSan.GioKetThuc,
        TrangThai = datSan.TrangThai,
        LyDoTuChoi = datSan.LyDoTuChoi,
        ThoiDiemNhanThucTe = datSan.ThoiDiemNhanThucTe,
        ThoiDiemTraThucTe = datSan.ThoiDiemTraThucTe,
        DichVuDaDung = datSan.SuDungDichVus
            .OrderByDescending(x => x.ThoiGianThem)
            .Select(x => new ChiTietDichVuSuDungViewModel
            {
                TenDichVu = x.DichVu?.TenDichVu ?? $"Dịch vụ #{x.MaDichVu}",
                DonViTinh = x.DichVu?.DonViTinh ?? string.Empty,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien,
                ThoiGianThem = x.ThoiGianThem
            })
            .ToList()
    };
}
