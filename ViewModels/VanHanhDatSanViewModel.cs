/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: ViewModel hiển thị danh sách và chi tiết đơn cho nghiệp vụ vận hành sân.
 */
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models.Enums;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class VanHanhDatSanViewModel
{
    public int MaDatSan { get; init; }
    public int MaKhachHang { get; init; }
    public string TenSan { get; init; } = string.Empty;
    public DateOnly NgaySuDung { get; init; }
    public TimeOnly GioBatDau { get; init; }
    public TimeOnly GioKetThuc { get; init; }
    public TrangThaiDatSan TrangThai { get; init; }
    public string? LyDoTuChoi { get; init; }
    public DateTime? ThoiDiemNhanThucTe { get; init; }
    public DateTime? ThoiDiemTraThucTe { get; init; }
    public IReadOnlyList<ChiTietDichVuSuDungViewModel> DichVuDaDung { get; init; } = [];
}

public class ChiTietDichVuSuDungViewModel
{
    public string TenDichVu { get; init; } = string.Empty;
    public string DonViTinh { get; init; } = string.Empty;
    public int SoLuong { get; init; }
    public decimal DonGia { get; init; }
    public decimal ThanhTien { get; init; }
    public DateTime ThoiGianThem { get; init; }
}
