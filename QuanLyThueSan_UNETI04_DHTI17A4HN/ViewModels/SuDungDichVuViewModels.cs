// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 4 - Dữ liệu hiển thị và biểu mẫu ghi nhận dịch vụ sử dụng.
using System.ComponentModel.DataAnnotations;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class ThemSuDungDichVuViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn dịch vụ.")]
    public int MaDichVu { get; set; }

    [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 đến 1000.")]
    public int SoLuong { get; set; } = 1;
}

public class SuDungDichVuChiTietViewModel
{
    public required DatSan Don { get; init; }
    public required IReadOnlyList<DichVu> DanhMuc { get; init; }
    public required IReadOnlyList<SuDungDichVu> PhatSinh { get; init; }
    public decimal TongTienDichVu => PhatSinh.Sum(x => x.ThanhTien);
}
