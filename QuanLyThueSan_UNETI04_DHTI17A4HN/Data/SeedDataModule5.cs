// Họ và tên: Trần Văn Lợi
// Mã sinh viên: 23103100237
// Nội dung thực hiện: Module 5 - Dữ liệu demo độc lập, bổ sung idempotent, giữ nguyên dữ liệu cũ
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;
namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Data;

public static class SeedDataModule5
{
    public static void Seed(ApplicationDbContext db)
    {
        var khach = db.KhachHangs.OrderBy(k => k.MaKhachHang).FirstOrDefault();
        var loai = db.LoaiSans.OrderBy(l => l.MaLoaiSan).FirstOrDefault();
        var khu = db.KhuVucs.OrderBy(k => k.MaKhuVuc).FirstOrDefault();
        if (khach == null || loai == null || khu == null) return;
        using var transaction = db.Database.BeginTransaction();
        var san = db.SanTheThaos.FirstOrDefault(s => s.TenSan == "Sân demo Module 5");
        if (san == null)
        {
            san = new SanTheThao { TenSan = "Sân demo Module 5", MaLoaiSan = loai.MaLoaiSan,
                MaKhuVuc = khu.MaKhuVuc, SucChua = 10, MoTa = "Dữ liệu mẫu thanh toán và thống kê Module 5" };
            db.SanTheThaos.Add(san);
            db.SaveChanges();
            db.BangGias.Add(new BangGia { MaSan = san.MaSan, TenBangGia = "Giá demo Module 5",
                NgayApDungTu = DateTime.Today.AddYears(-2), NgayApDungDen = DateTime.Today.AddYears(2),
                GioBatDau = TimeSpan.FromHours(6), GioKetThuc = TimeSpan.FromHours(23), DonGiaMoiGio = 200000 });
        }
        var dichVu = db.DichVus.FirstOrDefault(d => d.TenDichVu == "Nước uống demo Module 5");
        if (dichVu == null)
        {
            dichVu = new DichVu { TenDichVu = "Nước uống demo Module 5", DonViTinh = "Chai", DonGia = 10000 };
            db.DichVus.Add(dichVu);
        }
        db.SaveChanges();
        string[] trangThais = ["Chờ thanh toán", "Chờ thanh toán", "Chờ thanh toán", "Hoàn thành", "Hoàn thành", "Hoàn thành", "Đã hủy", "Đã xác nhận", "Đang sử dụng", "Chờ xác nhận"];
        for (int i = 0; i < trangThais.Length; i++)
        {
            var marker = $"MODULE5-DEMO-{i + 1:00}";
            if (db.DatSans.Any(d => d.GhiChu == marker)) continue;
            var ngay = i == 4 ? DateTime.Today.AddMonths(-1) : DateTime.Today.AddDays(-i);
            if (i >= 7) ngay = DateTime.Today;
            var batDau = i >= 7 ? 16 + (i - 7) * 2 : 8;
            var don = new DatSan { MaSan = san.MaSan, MaKhachHang = khach.MaKhachHang,
                NgaySuDung = ngay, NgayDat = ngay.AddDays(-1), GioBatDau = TimeSpan.FromHours(batDau),
                GioKetThuc = TimeSpan.FromHours(batDau + 2), SoNguoi = 4, TrangThai = trangThais[i], GhiChu = marker };
            db.DatSans.Add(don);
            db.SaveChanges();
            var coDichVu = i % 2 == 0;
            if (coDichVu)
                db.SuDungDichVus.Add(new SuDungDichVu { MaDatSan = don.MaDatSan, MaDichVu = dichVu.MaDichVu,
                    SoLuong = 2, DonGia = 10000, ThanhTien = 20000, ThoiGianThem = ngay.AddHours(batDau) });
            if (don.TrangThai == "Hoàn thành")
                db.ThanhToans.Add(new ThanhToan { MaDatSan = don.MaDatSan, TienSan = 400000,
                    TienDichVu = coDichVu ? 20000 : 0, TongTien = coDichVu ? 420000 : 400000,
                    PhuongThucThanhToan = i % 2 == 0 ? "Chuyển khoản" : "Tiền mặt",
                    NgayThanhToan = ngay.AddHours(10), TrangThaiThanhToan = "Đã thanh toán", GhiChu = marker });
            db.SaveChanges();
        }
        transaction.Commit();
    }
}
