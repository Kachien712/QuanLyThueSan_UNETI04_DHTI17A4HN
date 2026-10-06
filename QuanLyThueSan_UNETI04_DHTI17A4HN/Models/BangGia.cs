// Họ và tên: [Đào Duy Khánh]
// Mã sinh viên: [MSSV 23103100220]
// Nội dung thực hiện: Module 2 - Entity Bảng giá thuê sân theo khung giờ
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Models
{
    public class BangGia
    {
        [Key]
        public int MaBangGia { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sân")]
        public int MaSan { get; set; }

        [Required(ErrorMessage = "Tên bảng giá không được để trống")]
        [StringLength(100)]
        public string TenBangGia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giờ bắt đầu không được để trống")]
        public TimeSpan GioBatDau { get; set; }

        [Required(ErrorMessage = "Giờ kết thúc không được để trống")]
        public TimeSpan GioKetThuc { get; set; }

        [Range(1000, 100000000, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaMoiGio { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayApDungTu { get; set; }

        [DataType(DataType.Date)]
        public DateTime NgayApDungDen { get; set; }

        public bool TrangThai { get; set; } = true;

        [ForeignKey("MaSan")]
        public virtual SanTheThao? SanTheThao { get; set; }
    }
}
