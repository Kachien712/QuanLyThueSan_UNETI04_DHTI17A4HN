// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - ViewModel cho form Thêm mới và Chỉnh sửa Bảng giá theo khung giờ (Mục 6.2, 6.3 file Word)

using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels
{
    public class BangGiaFormViewModel
    {
        public int MaBangGia { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sân thể thao")]
        [Display(Name = "Sân thể thao")]
        public int MaSan { get; set; }

        [Required(ErrorMessage = "Tên bảng giá không được để trống")]
        [StringLength(100, ErrorMessage = "Tên bảng giá tối đa 100 ký tự")]
        [Display(Name = "Tên bảng giá")]
        public string TenBangGia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập giờ bắt đầu")]
        [Display(Name = "Giờ bắt đầu")]
        public TimeSpan GioBatDau { get; set; } = new TimeSpan(6, 0, 0);

        [Required(ErrorMessage = "Vui lòng nhập giờ kết thúc")]
        [Display(Name = "Giờ kết thúc")]
        public TimeSpan GioKetThuc { get; set; } = new TimeSpan(17, 0, 0);

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Range(1000, 100000000, ErrorMessage = "Đơn giá mỗi giờ phải lớn hơn 0")]
        [Display(Name = "Đơn giá / giờ (VNĐ)")]
        public decimal DonGiaMoiGio { get; set; } = 100000;

        [Required(ErrorMessage = "Vui lòng chọn ngày áp dụng từ")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày áp dụng từ")]
        public DateTime NgayApDungTu { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn ngày áp dụng đến")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày áp dụng đến")]
        public DateTime NgayApDungDen { get; set; } = DateTime.Today.AddYears(1);

        [Display(Name = "Trạng thái áp dụng")]
        public bool TrangThai { get; set; } = true;

        // Dropdown danh sách sân
        public SelectList? DanhSachSan { get; set; }

        // Tên sân để hiển thị khi xem theo ngữ cảnh một sân cụ thể
        public string? TenSanHienThi { get; set; }
    }
}
