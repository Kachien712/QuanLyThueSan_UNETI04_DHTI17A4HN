// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Kết quả thống kê LINQ theo sân, loại, khu vực, khách và dịch vụ
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class DongThongKe
{
    public int Ma { get; set; }
    public string Ten { get; set; } = "";
    public int SoLuot { get; set; }
    public decimal GiaTri { get; set; }
}

public class ThongKeViewModel
{
    public int TongDon { get; set; }
    public int DonHuy { get; set; }
    public decimal TyLeHuy => TongDon == 0 ? 0 : DonHuy * 100m / TongDon;
    public List<DongThongKe> LuotTheoSan { get; set; } = [];
    public List<DongThongKe> LuotTheoLoai { get; set; } = [];
    public List<DongThongKe> LuotTheoKhuVuc { get; set; } = [];
    public List<DongThongKe> GioTheoSan { get; set; } = [];
    public List<DongThongKe> ThuTheoSan { get; set; } = [];
    public List<DongThongKe> ThuTheoLoai { get; set; } = [];
    public List<DongThongKe> ThuTheoThang { get; set; } = [];
    public List<DongThongKe> KhungGio { get; set; } = [];
    public List<DongThongKe> KhachHang { get; set; } = [];
    public List<DongThongKe> DichVu { get; set; } = [];
}
