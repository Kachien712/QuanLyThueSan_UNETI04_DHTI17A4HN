// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - ViewModel tra cứu và tìm sân trống theo khung thời gian (LINQ Server-side)

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class TimSanTrongViewModel
    {
        // 1. Tiêu chí tìm kiếm thời gian bắt buộc
        [Required(ErrorMessage = "Vui lòng chọn ngày sử dụng")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sử dụng")]
        public DateTime NgaySuDung { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu")]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan GioBatDau { get; set; } = new TimeSpan(8, 0, 0); // 08:00

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc")]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan GioKetThuc { get; set; } = new TimeSpan(9, 30, 0); // 09:30

        // 2. Tiêu chí bộ lọc tùy chọn
        [Display(Name = "Loại sân")]
        public int? MaLoaiSan { get; set; }

        [Display(Name = "Khu vực")]
        public int? MaKhuVuc { get; set; }

        [Display(Name = "Số người tham gia")]
        [Range(1, 200, ErrorMessage = "Số người phải lớn hơn 0")]
        public int? SoNguoiYeuCau { get; set; }

        // 3. DropDown phục vụ bộ lọc tìm kiếm
        public SelectList? DanhSachLoaiSan { get; set; }
        public SelectList? DanhSachKhuVuc { get; set; }

        // 4. Kết quả tìm kiếm trả về các sân trống khả dụng
        public List<SanTrongItemViewModel> DanhSachSanTrong { get; set; } = new List<SanTrongItemViewModel>();

        // Cờ trạng thái xác định người dùng đã bấm tìm kiếm hay mới tải trang
        public bool DaTimKiem { get; set; } = false;

        public int TongSoSanTrong => DanhSachSanTrong.Count;
    }

    // Lớp chứa thông tin từng sân trống tìm được để hiển thị trực quan dạng Card
    public class SanTrongItemViewModel
    {
        public int MaSan { get; set; }
        public string TenSan { get; set; } = string.Empty;
        public string TenLoaiSan { get; set; } = string.Empty;
        public string TenKhuVuc { get; set; } = string.Empty;
        public int SoNguoiToiDa { get; set; }
        public decimal? GiaThamKhao { get; set; }
        public string? GhiChu { get; set; }
    }
}
