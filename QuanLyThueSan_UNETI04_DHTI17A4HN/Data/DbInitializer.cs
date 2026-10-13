// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Lớp điều phối khởi tạo CSDL dùng chung cho cả nhóm, gọi seed data của các thành viên

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Đảm bảo CSDL đã được tạo
            context.Database.EnsureCreated();

            // 1. Module 2 (Đào Duy Khánh - 23103100220):
            // Nạp dữ liệu mẫu: 4 Loại sân, 4 Khu vực, 18 Sân thể thao và Bảng giá theo khung giờ (Mục 16 file Word)
            SeedData_23103100220_DaoDuyKhanh.Seed(context);

            // Các thành viên khác bổ sung dữ liệu seed của module tương ứng tại đây khi tích hợp
        }
    }
}
