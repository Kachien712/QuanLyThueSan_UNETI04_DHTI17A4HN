// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Gọi seed demo riêng sau dữ liệu các module
// Họ và tên: Đào Duy Khánh - MSSV: 23103100220 (Module 2 - Sân & Bảng giá)
// Đào Đức Kiên - MSSV: 23103100222 (Module 3 - 25 Khách hàng)
// Khuất Anh Chiến - MSSV: 23103100234 (Module 1 - Tài khoản Admin/Nhân viên)
// Nội dung thực hiện: Lớp điều phối khởi tạo CSDL dùng chung cho cả nhóm (Mục 16 file Word)

using QuanLyThueSan_UNETI04_DHTI17A4HN.Helpers;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Đảm bảo CSDL đã được tạo
            context.Database.EnsureCreated();

            // 1. MODULE 2 (Đào Duy Khánh - 23103100220):
            // Nạp dữ liệu mẫu: 4 Loại sân, 4 Khu vực, 18 Sân thể thao và Bảng giá theo khung giờ
            SeedData_23103100220_DaoDuyKhanh.Seed(context);

            // 2. MODULE 3 (Đào Đức Kiên - 23103100222):
            // Nạp dữ liệu mẫu 25 khách hàng phục vụ kiểm thử phân trang
            SeedData_23103100222_DaoDucKien.Seed(context);

            // 3. MODULE 1 (Khuất Anh Chiến - 23103100234):
            // Tài khoản quản trị mặc định để kiểm thử phân quyền (Mục 16 file Word)
            if (!context.TaiKhoans.Any(t => t.VaiTro == "Admin"))
            {
                context.TaiKhoans.AddRange(
                    new TaiKhoan
                    {
                        TenDangNhap = "admin",
                        MatKhau = MatKhauHelper.MaHoa("123456"),
                        HoTen = "Quản trị viên",
                        Email = "admin@quanyttt.edu.vn",
                        VaiTro = "Admin",
                        TrangThai = true
                    },
                    new TaiKhoan
                    {
                        TenDangNhap = "nhanvien",
                        MatKhau = MatKhauHelper.MaHoa("123456"),
                        HoTen = "Nhân viên trực sân",
                        Email = "nhanvien@quanyttt.edu.vn",
                        VaiTro = "NhanVien",
                        TrangThai = true
                    });
                context.SaveChanges();
            }

            // Khách hàng chưa liên kết tài khoản (MaTaiKhoan null) -> tạo tài khoản và ghép mã
            var khachChuaCoTaiKhoan = context.KhachHangs
                .Where(k => k.MaTaiKhoan == null)
                .ToList();

            foreach (var khach in khachChuaCoTaiKhoan)
            {
                string tenDangNhap;
                if (!string.IsNullOrWhiteSpace(khach.Email)
                    && !context.TaiKhoans.Any(t => t.TenDangNhap == khach.Email))
                {
                    tenDangNhap = khach.Email!;
                }
                else
                {
                    tenDangNhap = $"kh{khach.MaKhachHang}";
                    int soThuTu = 1;
                    while (context.TaiKhoans.Any(t => t.TenDangNhap == tenDangNhap))
                    {
                        tenDangNhap = $"kh{khach.MaKhachHang}_{soThuTu++}";
                    }
                }

                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = tenDangNhap,
                    MatKhau = MatKhauHelper.MaHoa("123456"),
                    HoTen = khach.HoTen,
                    Email = khach.Email ?? $"kh{khach.MaKhachHang}@khachhang.vn",
                    VaiTro = "KhachHang",
                    TrangThai = khach.TrangThai
                };
                context.TaiKhoans.Add(taiKhoan);
                context.SaveChanges();

                khach.MaTaiKhoan = taiKhoan.MaTaiKhoan;
            }

            if (khachChuaCoTaiKhoan.Count > 0)
            {
                context.SaveChanges();
            }
            SeedDataModule5.Seed(context);
        }
    }
}
