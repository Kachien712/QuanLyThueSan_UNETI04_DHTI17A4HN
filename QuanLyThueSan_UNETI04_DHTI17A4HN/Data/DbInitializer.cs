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
            // Tự động kiểm tra và khởi tạo dữ liệu mẫu cho bảng Khách hàng
            if (!context.KhachHangs.Any())
            {
                var danhSachKhachHang = new List<KhachHang>
                {
                    new KhachHang { HoTen = "Nguyễn Văn An", SoDienThoai = "0901234501", Email = "nguyenvanan@gmail.com", DiaChi = "456 Minh Khai, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Trần Thị Bích", SoDienThoai = "0901234502", Email = "tranthibich@gmail.com", DiaChi = "123 Lĩnh Nam, Hoàng Mai, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Lê Hoàng Cường", SoDienThoai = "0901234503", Email = "lehoangcuong@gmail.com", DiaChi = "78 Bạch Mai, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Phạm Quang Dũng", SoDienThoai = "0901234504", Email = "phamquangdung@gmail.com", DiaChi = "89 Giải Phóng, Đống Đa, Hà Nội", TrangThai = false },
                    new KhachHang { HoTen = "Vũ Thị Em", SoDienThoai = "0901234505", Email = "vuthiem@gmail.com", DiaChi = "12 Đại La, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Đỗ Văn Giang", SoDienThoai = "0901234506", Email = "dovangiang@gmail.com", DiaChi = "34 Trường Chinh, Thanh Xuân, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Hoàng Thị Hoa", SoDienThoai = "0901234507", Email = "hoangthihoa@gmail.com", DiaChi = "56 Tam Trinh, Hoàng Mai, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Bùi Văn Hùng", SoDienThoai = "0901234508", Email = "buivanhung@gmail.com", DiaChi = "67 Định Công, Hoàng Mai, Hà Nội", TrangThai = false },
                    new KhachHang { HoTen = "Ngô Thị Kim", SoDienThoai = "0901234509", Email = "ngothikim@gmail.com", DiaChi = "23 Nguyễn An Ninh, Hoàng Mai, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Đặng Văn Lâm", SoDienThoai = "0901234510", Email = "dangvanlam@gmail.com", DiaChi = "90 Phố Vọng, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Dương Quốc Mạnh", SoDienThoai = "0901234511", Email = "duongquocmanh@gmail.com", DiaChi = "101 Kim Ngưu, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Lý Thị Nga", SoDienThoai = "0901234512", Email = "lythinga@gmail.com", DiaChi = "15 Tạ Quang Bửu, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Mai Văn Phong", SoDienThoai = "0901234513", Email = "maivanphong@gmail.com", DiaChi = "88 Lê Thanh Nghị, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Đinh Thị Quỳnh", SoDienThoai = "0901234514", Email = "dinhthiquynh@gmail.com", DiaChi = "42 Lạc Trung, Hai Bà Trưng, Hà Nội", TrangThai = false },
                    new KhachHang { HoTen = "Hồ Văn Sơn", SoDienThoai = "0901234515", Email = "hovanson@gmail.com", DiaChi = "75 Tân Mai, Hoàng Mai, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Võ Thị Thanh", SoDienThoai = "0901234516", Email = "vothithanh@gmail.com", DiaChi = "19 Trương Định, Hoàng Mai, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Phan Văn Thắng", SoDienThoai = "0901234517", Email = "phanvanthang@gmail.com", DiaChi = "33 Giáp Bát, Hoàng Mai, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Trịnh Thị Uyên", SoDienThoai = "0901234518", Email = "trinhthiuyen@gmail.com", DiaChi = "50 Vĩnh Tuy, Hai Bà Trưng, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Lương Văn Vũ", SoDienThoai = "0901234519", Email = "luongvanvu@gmail.com", DiaChi = "82 Khương Trung, Thanh Xuân, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Cao Thị Yến", SoDienThoai = "0901234520", Email = "caothiyen@gmail.com", DiaChi = "99 Chùa Bộc, Đống Đa, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Nguyễn Minh Khang", SoDienThoai = "0901234521", Email = "nguyenminhkhang@gmail.com", DiaChi = "11 Tây Sơn, Đống Đa, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Trần Tuấn Kiệt", SoDienThoai = "0901234522", Email = "trantuankiet@gmail.com", DiaChi = "22 Hoàng Cầu, Đống Đa, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Phạm Gia Hưng", SoDienThoai = "0901234523", Email = "phamgiahung@gmail.com", DiaChi = "63 Thái Hà, Đống Đa, Hà Nội", TrangThai = false },
                    new KhachHang { HoTen = "Lê Bảo Nam", SoDienThoai = "0901234524", Email = "lebaonam@gmail.com", DiaChi = "47 Huỳnh Thúc Kháng, Đống Đa, Hà Nội", TrangThai = true },
                    new KhachHang { HoTen = "Đào Đức Kiên", SoDienThoai = "0901234525", Email = "daoduckien@gmail.com", DiaChi = "218 Lĩnh Nam, Hoàng Mai, Hà Nội", TrangThai = true }
                };

                context.KhachHangs.AddRange(danhSachKhachHang);
                context.SaveChanges();
            }

            // ===== PHẦN BỔ SUNG - MODULE 1 (Khuất Anh Chiến - 23103100234) =====

            // 1. Tài khoản quản trị mặc định để kiểm thử phân quyền (Mục 16 file Word)
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

            // 2. Khách hàng chưa liên kết tài khoản (MaTaiKhoan null) -> tạo tài khoản và ghép mã
            var khachChuaCoTaiKhoan = context.KhachHangs
                .Where(k => k.MaTaiKhoan == null)
                .ToList();

            foreach (var khach in khachChuaCoTaiKhoan)
            {
                // Ưu tiên dùng email làm tên đăng nhập; email null hoặc đã bị chiếm thì dùng mã khách hàng
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
                context.SaveChanges(); // Lưu ngay để lấy MaTaiKhoan

                khach.MaTaiKhoan = taiKhoan.MaTaiKhoan;
            }

            if (khachChuaCoTaiKhoan.Count > 0)
            {
                context.SaveChanges();
            }
        }
    }
}
