// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Các chỉ số tổng quan và lịch sân trong ngày
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class DashboardViewModel
{
    public DateTime Ngay { get; set; }
    public int TongSoSan { get; set; }
    public Dictionary<string, int> SanTheoTinhTrang { get; set; } = [];
    public Dictionary<string, int> DonTheoTrangThai { get; set; } = [];
    public int SoLuotDungTrongNgay { get; set; }
    public int SoDonDaHuy { get; set; }
    public decimal DoanhThuNgay { get; set; }
    public decimal DoanhThuThang { get; set; }
    public List<DatSan> LichSapToi { get; set; } = [];
}
