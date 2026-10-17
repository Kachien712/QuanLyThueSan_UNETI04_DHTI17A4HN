// Họ và tên: Lê Ngọc Hải Nam
// Mã sinh viên: 23103100219
// Nội dung thực hiện: Module 4 tuần 3 - dữ liệu biểu mẫu danh mục dịch vụ.
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.ViewModels;

public class DichVuFormViewModel
{
    public int MaDichVu { get; set; }

    [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
    [StringLength(100)]
    [Display(Name = "Tên dịch vụ")]
    public string TenDichVu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Đơn vị tính không được để trống")]
    [StringLength(50)]
    [Display(Name = "Đơn vị tính")]
    public string DonViTinh { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "100000000", ErrorMessage = "Đơn giá phải từ 0 đến 100.000.000")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm")]
    [Display(Name = "Số lượng tồn (để trống nếu không quản lý)")]
    public int? SoLuongTon { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;
}
