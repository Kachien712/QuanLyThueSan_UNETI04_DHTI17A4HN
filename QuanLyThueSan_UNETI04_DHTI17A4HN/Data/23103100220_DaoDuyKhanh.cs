// Họ và tên: Đào Duy Khánh
// Mã sinh viên: 23103100220
// Nội dung thực hiện: Module 2 - Khởi tạo dữ liệu mẫu Loại sân, Khu vực, 18 Sân thể thao và Bảng giá theo khung giờ (Mục 16 file Word)

using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Data
{
    public static class SeedData_23103100220_DaoDuyKhanh
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Đảm bảo CSDL đã sẵn sàng
            context.Database.EnsureCreated();

            // 1. DỮ LIỆU MẪU: LOẠI SÂN (Tối thiểu 4 loại sân theo Mục 16)
            if (!context.LoaiSans.Any())
            {
                var danhSachLoaiSan = new List<LoaiSan>
                {
                    new LoaiSan { TenLoaiSan = "Sân bóng đá cỏ nhân tạo", MoTa = "Sân bóng đá mini 5-7 người tiêu chuẩn", SoNguoiToiDa = 14, TrangThai = true },
                    new LoaiSan { TenLoaiSan = "Sân cầu lông", MoTa = "Sân cầu lông trong nhà thảm tiêu chuẩn thi đấu", SoNguoiToiDa = 4, TrangThai = true },
                    new LoaiSan { TenLoaiSan = "Sân tennis", MoTa = "Sân quần vợt mặt cứng đạt chuẩn quốc tế", SoNguoiToiDa = 4, TrangThai = true },
                    new LoaiSan { TenLoaiSan = "Sân bóng rổ", MoTa = "Sân bóng rổ tiêu chuẩn nửa sân và cả sân", SoNguoiToiDa = 10, TrangThai = true }
                };
                context.LoaiSans.AddRange(danhSachLoaiSan);
                context.SaveChanges();
            }

            // 2. DỮ LIỆU MẪU: KHU VỰC (Tối thiểu 3 - 4 khu vực theo Mục 16)
            if (!context.KhuVucs.Any())
            {
                var danhSachKhuVuc = new List<KhuVuc>
                {
                    new KhuVuc { TenKhuVuc = "Khu A - Sân ngoài trời", ViTri = "Khuôn viên phía Tây", MoTa = "Cụm sân ngoài trời thoáng mát có đèn chiếu sáng cao áp", TrangThai = true },
                    new KhuVuc { TenKhuVuc = "Khu B - Sân ngoài trời", ViTri = "Khuôn viên phía Đông", MoTa = "Cụm sân cỏ nhân tạo có mái che lưới giảm nắng", TrangThai = true },
                    new KhuVuc { TenKhuVuc = "Khu C - Nhà thi đấu đa năng", ViTri = "Tòa nhà thể chất tầng 1-2", MoTa = "Nhà thi đấu có hệ thống thông gió và sàn gỗ công nghiệp", TrangThai = true },
                    new KhuVuc { TenKhuVuc = "Khu VIP - Sân chuyên gia", ViTri = "Khu liên hợp thể thao VIP", MoTa = "Khu vực sân cao cấp có phòng thay đồ riêng biệt", TrangThai = true }
                };
                context.KhuVucs.AddRange(danhSachKhuVuc);
                context.SaveChanges();
            }

            // Lấy ID thực tế từ DB
            var loaiBongDa = context.LoaiSans.FirstOrDefault(l => l.TenLoaiSan.Contains("bóng đá"))?.MaLoaiSan ?? 1;
            var loaiCauLong = context.LoaiSans.FirstOrDefault(l => l.TenLoaiSan.Contains("cầu lông"))?.MaLoaiSan ?? 2;
            var loaiTennis = context.LoaiSans.FirstOrDefault(l => l.TenLoaiSan.Contains("tennis"))?.MaLoaiSan ?? 3;
            var loaiBongRo = context.LoaiSans.FirstOrDefault(l => l.TenLoaiSan.Contains("bóng rổ"))?.MaLoaiSan ?? 4;

            var khuA = context.KhuVucs.FirstOrDefault(k => k.TenKhuVuc.Contains("Khu A"))?.MaKhuVuc ?? 1;
            var khuB = context.KhuVucs.FirstOrDefault(k => k.TenKhuVuc.Contains("Khu B"))?.MaKhuVuc ?? 2;
            var khuC = context.KhuVucs.FirstOrDefault(k => k.TenKhuVuc.Contains("Khu C"))?.MaKhuVuc ?? 3;
            var khuVip = context.KhuVucs.FirstOrDefault(k => k.TenKhuVuc.Contains("VIP"))?.MaKhuVuc ?? 4;

            // 3. DỮ LIỆU MẪU: SÂN THỂ THAO (18 sân đa dạng môn và tình trạng)
            if (!context.SanTheThaos.Any())
            {
                var danhSachSan = new List<SanTheThao>
                {
                    // Sân bóng đá (Khu A & B)
                    new SanTheThao { TenSan = "Sân Bóng Đá Mini A1", MaLoaiSan = loaiBongDa, MaKhuVuc = khuA, SucChua = 14, MoTa = "Mặt cỏ nhân tạo cao cấp, đèn LED chiếu sáng ban đêm", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Bóng Đá Mini A2", MaLoaiSan = loaiBongDa, MaKhuVuc = khuA, SucChua = 14, MoTa = "Sân 7 người, khán đài mini bên cạnh", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Bóng Đá Mini A3", MaLoaiSan = loaiBongDa, MaKhuVuc = khuA, SucChua = 10, MoTa = "Sân 5 người tiêu chuẩn cho sinh viên", TinhTrang = "Tạm ngừng" },
                    new SanTheThao { TenSan = "Sân Bóng Đá B1", MaLoaiSan = loaiBongDa, MaKhuVuc = khuB, SucChua = 14, MoTa = "Sân cỏ nhân tạo mới thay mặt cỏ", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Bóng Đá B2", MaLoaiSan = loaiBongDa, MaKhuVuc = khuB, SucChua = 14, MoTa = "Khu vực khán đài có mái che", TinhTrang = "Đang bảo trì" },

                    // Sân Cầu lông (Khu C & VIP)
                    new SanTheThao { TenSan = "Sân Cầu Lông C1", MaLoaiSan = loaiCauLong, MaKhuVuc = khuC, SucChua = 4, MoTa = "Thảm PVC chống trơn trượt, ánh sáng tiêu chuẩn", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Cầu Lông C2", MaLoaiSan = loaiCauLong, MaKhuVuc = khuC, SucChua = 4, MoTa = "Sân trong nhà điều hòa mát mẻ", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Cầu Lông C3", MaLoaiSan = loaiCauLong, MaKhuVuc = khuC, SucChua = 4, MoTa = "Sân luyện tập câu lạc bộ", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Cầu Lông C4", MaLoaiSan = loaiCauLong, MaKhuVuc = khuC, SucChua = 4, MoTa = "Sân đang bảo dưỡng lưới và đèn", TinhTrang = "Đang bảo trì" },
                    new SanTheThao { TenSan = "Sân Cầu Lông VIP 01", MaLoaiSan = loaiCauLong, MaKhuVuc = khuVip, SucChua = 4, MoTa = "Phòng thi đấu riêng biệt, máy lạnh và nước uống", TinhTrang = "Đang hoạt động" },

                    // Sân Tennis (Khu A & VIP)
                    new SanTheThao { TenSan = "Sân Tennis A1", MaLoaiSan = loaiTennis, MaKhuVuc = khuA, SucChua = 4, MoTa = "Mặt sân cứng sơn Decoturf đạt chuẩn ATP", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Tennis A2", MaLoaiSan = loaiTennis, MaKhuVuc = khuA, SucChua = 4, MoTa = "Sân có hệ thống chắn gió xung quanh", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Tennis VIP Pro", MaLoaiSan = loaiTennis, MaKhuVuc = khuVip, SucChua = 4, MoTa = "Sân thi đấu chuyên nghiệp kèm huấn luyện viên", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Tennis A3", MaLoaiSan = loaiTennis, MaKhuVuc = khuA, SucChua = 4, MoTa = "Sân chuẩn bị quy hoạch lại mặt cỏ", TinhTrang = "Ngừng sử dụng" },

                    // Sân Bóng rổ (Khu B & C)
                    new SanTheThao { TenSan = "Sân Bóng Rổ B1", MaLoaiSan = loaiBongRo, MaKhuVuc = khuB, SucChua = 10, MoTa = "Sân bóng rổ ngoài trời mặt bê tông sơn nhám", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Bóng Rổ B2", MaLoaiSan = loaiBongRo, MaKhuVuc = khuB, SucChua = 10, MoTa = "Sân bóng rổ 3x3 có đèn đêm", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Bóng Rổ C1 (Trong nhà)", MaLoaiSan = loaiBongRo, MaKhuVuc = khuC, SucChua = 10, MoTa = "Sàn gỗ công nghiệp, bảng rổ kính cường lực", TinhTrang = "Đang hoạt động" },
                    new SanTheThao { TenSan = "Sân Bóng Rổ C2 (Trong nhà)", MaLoaiSan = loaiBongRo, MaKhuVuc = khuC, SucChua = 10, MoTa = "Sân trong nhà phục vụ giải đấu sinh viên", TinhTrang = "Tạm ngừng" }
                };

                context.SanTheThaos.AddRange(danhSachSan);
                context.SaveChanges();
            }

            // 4. DỮ LIỆU MẪU: BẢNG GIÁ THUÊ SÂN THEO KHUNG GIỜ (Mục 6.2, 16)
            if (!context.BangGias.Any())
            {
                var dsSan = context.SanTheThaos.ToList();
                var danhSachBangGia = new List<BangGia>();

                foreach (var san in dsSan)
                {
                    decimal giaNgay = san.SucChua > 10 ? 250000 : 120000;
                    decimal giaToi = san.SucChua > 10 ? 350000 : 180000;

                    // Khung ngày: 06:00 -> 17:00
                    danhSachBangGia.Add(new BangGia
                    {
                        MaSan = san.MaSan,
                        TenBangGia = $"Khung ngày ({san.TenSan})",
                        GioBatDau = new TimeSpan(6, 0, 0),
                        GioKetThuc = new TimeSpan(17, 0, 0),
                        DonGiaMoiGio = giaNgay,
                        NgayApDungTu = new DateTime(2026, 1, 1),
                        NgayApDungDen = new DateTime(2026, 12, 31),
                        TrangThai = true
                    });

                    // Khung tối giờ vàng: 17:00 -> 22:00
                    danhSachBangGia.Add(new BangGia
                    {
                        MaSan = san.MaSan,
                        TenBangGia = $"Khung tối giờ vàng ({san.TenSan})",
                        GioBatDau = new TimeSpan(17, 0, 0),
                        GioKetThuc = new TimeSpan(22, 0, 0),
                        DonGiaMoiGio = giaToi,
                        NgayApDungTu = new DateTime(2026, 1, 1),
                        NgayApDungDen = new DateTime(2026, 12, 31),
                        TrangThai = true
                    });
                }

                context.BangGias.AddRange(danhSachBangGia);
                context.SaveChanges();
            }
        }
    }
}
