// Họ và tên: Nhóm 4 (Trưởng nhóm phụ trách)
// Đề tài 18: Quản lý sân thể thao và đặt lịch sử dụng sân
// Nội dung thực hiện: Lớp DbContext quản lý CSDL Entity Framework Core 10
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Khai báo 10 DbSet tương ứng với 10 Entity
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<LoaiSan> LoaiSans { get; set; }
        public DbSet<KhuVuc> KhuVucs { get; set; }
        public DbSet<SanTheThao> SanTheThaos { get; set; }
        public DbSet<BangGia> BangGias { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DatSan> DatSans { get; set; }
        public DbSet<DichVu> DichVus { get; set; }
        public DbSet<SuDungDichVu> SuDungDichVus { get; set; }
        public DbSet<ThanhToan> ThanhToans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Ràng buộc UNIQUE không trùng lặp (Mục 5, 8, 13 file Word)
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap).IsUnique();

            modelBuilder.Entity<LoaiSan>()
                .HasIndex(l => l.TenLoaiSan).IsUnique();

            modelBuilder.Entity<KhuVuc>()
                .HasIndex(k => k.TenKhuVuc).IsUnique();

            modelBuilder.Entity<DichVu>()
                .HasIndex(d => d.TenDichVu).IsUnique();

            // 2. Quan hệ 1-1 giữa DatSan và ThanhToan (Mục 10 file Word)
            modelBuilder.Entity<ThanhToan>()
                .HasIndex(t => t.MaDatSan).IsUnique();

            // 3. Cấu hình bảo toàn toàn vẹn dữ liệu (Restrict Delete để không làm mất lịch sử đặt sân)
            modelBuilder.Entity<DatSan>()
                .HasOne(d => d.SanTheThao)
                .WithMany(s => s.DatSans)
                .HasForeignKey(d => d.MaSan)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatSan>()
                .HasOne(d => d.KhachHang)
                .WithMany(k => k.DatSans)
                .HasForeignKey(d => d.MaKhachHang)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SuDungDichVu>()
                .HasOne(s => s.DichVu)
                .WithMany(d => d.SuDungDichVus)
                .HasForeignKey(s => s.MaDichVu)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
