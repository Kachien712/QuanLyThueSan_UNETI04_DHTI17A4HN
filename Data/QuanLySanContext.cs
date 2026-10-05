/*
 * Họ và tên: Lê Ngọc Hải Nam
 * Mã sinh viên: 23103100219
 * Nội dung thực hiện: Khai báo DbSet và ràng buộc dữ liệu phục vụ xác nhận, nhận/trả sân và dịch vụ sử dụng.
 */
using Microsoft.EntityFrameworkCore;
using QuanLyThueSan_UNETI04_DHTI17A4HN.Models;

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Data;

public class QuanLySanContext(DbContextOptions<QuanLySanContext> options) : DbContext(options)
{
    public DbSet<SanTheThao> SanTheThaos => Set<SanTheThao>();
    public DbSet<DatSan> DatSans => Set<DatSan>();
    public DbSet<DichVu> DichVus => Set<DichVu>();
    public DbSet<SuDungDichVu> SuDungDichVus => Set<SuDungDichVu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasIndex(x => x.TenDichVu).IsUnique();
            entity.Property(x => x.DonGia).HasPrecision(18, 2);
        });

        modelBuilder.Entity<DatSan>(entity =>
        {
            entity.Property(x => x.DonGiaSanDuKien).HasPrecision(18, 2);
            entity.HasIndex(x => new { x.MaSan, x.NgaySuDung, x.TrangThai });
            entity.HasOne(x => x.San)
                .WithMany(x => x.DatSans)
                .HasForeignKey(x => x.MaSan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SuDungDichVu>(entity =>
        {
            entity.Property(x => x.DonGia).HasPrecision(18, 2);
            entity.Property(x => x.ThanhTien).HasPrecision(18, 2);
            entity.HasOne(x => x.DatSan)
                .WithMany(x => x.SuDungDichVus)
                .HasForeignKey(x => x.MaDatSan)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.DichVu)
                .WithMany(x => x.SuDungDichVus)
                .HasForeignKey(x => x.MaDichVu)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
