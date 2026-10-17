// Họ và tên: Đào Đức Kiên
// Mã sinh viên: 23103100222
// Nội dung thực hiện: Module 3 - ViewModel tạo mới đơn đặt sân kèm Validation nghiệp vụ và hỗ trợ chọn sân

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class DatSanCreateViewModel
    {
        // 1. Thông tin khách hàng đặt sân
        [Required(ErrorMessage = "Vui lòng chọn khách hàng")]
        [Display(Name = "Khách hàng")]
        public int MaKhachHang { get; set; }

        // 2. Thông tin sân được chọn
        [Required(ErrorMessage = "Vui lòng chọn sân")]
        [Display(Name = "Sân thể thao")]
        public int MaSan { get; set; }

        // 3. Thông tin thời gian sử dụng
        [Required(ErrorMessage = "Vui lòng chọn ngày sử dụng")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sử dụng")]
        public DateTime NgaySuDung { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu")]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan GioBatDau { get; set; } = new TimeSpan(8, 0, 0); // Mặc định 08:00

        [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc")]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan GioKetThuc { get; set; } = new TimeSpan(9, 30, 0); // Mặc định 09:30

        // 4. Số người tham gia (phục vụ kiểm tra sức chứa tối đa của loại sân)
        [Required(ErrorMessage = "Vui lòng nhập số người tham gia")]
        [Range(1, 200, ErrorMessage = "Số người tham gia phải lớn hơn 0 và không vượt quá 200")]
        [Display(Name = "Số người tham gia")]
        public int SoNguoi { get; set; } = 10;

        // 5. Ghi chú của khách hàng
        [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự")]
        [Display(Name = "Ghi chú yêu cầu")]
        public string? GhiChu { get; set; }

        // 6. Danh sách DropDown hỗ trợ chọn trên giao diện
        public SelectList? DanhSachKhachHang { get; set; }
        public SelectList? DanhSachSan { get; set; }

        // 7. Thông tin hiển thị bổ trợ khi người dùng chọn sân (tên loại sân, khu vực, sức chứa)
        public string? TenSanDuocChon { get; set; }
        public string? TenLoaiSan { get; set; }
        public string? TenKhuVuc { get; set; }
        public int? SoNguoiToiDa { get; set; }
    }
}
