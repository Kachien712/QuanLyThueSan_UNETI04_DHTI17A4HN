/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Entity dịch vụ, dữ liệu nguồn để thêm dịch vụ cho lượt sử dụng sân.
 */
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

public class DichVu
{
    [Key]
    public int MaDichVu { get; set; }

    [Required(ErrorMessage = "Tên dịch vụ là bắt buộc.")]
    [StringLength(150)]
    [Display(Name = "Tên dịch vụ")]
    public string TenDichVu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Đơn vị tính là bắt buộc.")]
    [StringLength(50)]
    [Display(Name = "Đơn vị tính")]
    public string DonViTinh { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0.")]
    [Display(Name = "Đơn giá")]
    public decimal DonGia { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm.")]
    [Display(Name = "Số lượng tồn")]
    public int? SoLuongTon { get; set; }

    [Display(Name = "Đang hoạt động")]
    public bool TrangThai { get; set; } = true;

    public ICollection<SuDungDichVu> SuDungDichVus { get; set; } = new List<SuDungDichVu>();
}
