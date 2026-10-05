/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Khởi tạo Entity dùng chung tối thiểu để Module 4 kiểm tra sân còn khả dụng khi xác nhận đơn.
 */
using System.ComponentModel.DataAnnotations;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models.Enums;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

// Entity dùng chung tối thiểu để Module 4 kiểm tra sân còn khả dụng.
// Thành viên phụ trách Module 2 sẽ bổ sung các thuộc tính quản lý sân còn lại.
public class SanTheThao
{
    [Key]
    public int MaSan { get; set; }

    [Required, StringLength(150)]
    public string TenSan { get; set; } = string.Empty;

    public TinhTrangSan TinhTrang { get; set; } = TinhTrangSan.DangHoatDong;

    public ICollection<DatSan> DatSans { get; set; } = new List<DatSan>();
}
