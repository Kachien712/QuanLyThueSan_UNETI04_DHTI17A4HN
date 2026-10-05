using System.ComponentModel.DataAnnotations;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models.Enums;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

// Entity dùng chung tối thiểu để Module 4 xử lý vòng đời đơn.
// Thành viên phụ trách Module 3 sẽ thống nhất và bổ sung các thuộc tính đặt sân còn lại.
public class DatSan
{
    [Key]
    public int MaDatSan { get; set; }

    [Required]
    public int MaKhachHang { get; set; }

    [Required]
    public int MaSan { get; set; }

    [DataType(DataType.Date)]
    public DateOnly NgaySuDung { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly GioBatDau { get; set; }

    [DataType(DataType.Time)]
    public TimeOnly GioKetThuc { get; set; }

    public DateTime NgayDat { get; set; } = DateTime.Now;

    [Range(0.01, double.MaxValue)]
    public decimal DonGiaSanDuKien { get; set; }

    public TrangThaiDatSan TrangThai { get; set; } = TrangThaiDatSan.ChoXacNhan;

    [StringLength(500)]
    public string? LyDoTuChoi { get; set; }

    public DateTime? ThoiDiemNhanThucTe { get; set; }

    public DateTime? ThoiDiemTraThucTe { get; set; }

    public SanTheThao? San { get; set; }

    public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
}
