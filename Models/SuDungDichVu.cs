/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Entity lưu dịch vụ phát sinh, chốt đơn giá và thành tiền tại thời điểm sử dụng.
 */
using System.ComponentModel.DataAnnotations;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

public class SuDungDichVu
{
    [Key]
    public int MaSuDung { get; set; }

    [Required]
    public int MaDatSan { get; set; }

    [Required]
    public int MaDichVu { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0.")]
    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public decimal ThanhTien { get; set; }

    public DateTime ThoiGianThem { get; set; } = DateTime.Now;

    public DatSan? DatSan { get; set; }

    public DichVu? DichVu { get; set; }
}
