using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTaoModule4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DichVus",
                columns: table => new
                {
                    MaDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDichVu = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "SanTheThaos",
                columns: table => new
                {
                    MaSan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSan = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TinhTrang = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanTheThaos", x => x.MaSan);
                });

            migrationBuilder.CreateTable(
                name: "DatSans",
                columns: table => new
                {
                    MaDatSan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaSan = table.Column<int>(type: "int", nullable: false),
                    NgaySuDung = table.Column<DateOnly>(type: "date", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DonGiaSanDuKien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TrangThai = table.Column<int>(type: "int", nullable: false),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ThoiDiemNhanThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiDiemTraThucTe = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatSans", x => x.MaDatSan);
                    table.ForeignKey(
                        name: "FK_DatSans_SanTheThaos_MaSan",
                        column: x => x.MaSan,
                        principalTable: "SanTheThaos",
                        principalColumn: "MaSan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SuDungDichVus",
                columns: table => new
                {
                    MaSuDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatSan = table.Column<int>(type: "int", nullable: false),
                    MaDichVu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ThoiGianThem = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuDungDichVus", x => x.MaSuDung);
                    table.ForeignKey(
                        name: "FK_SuDungDichVus_DatSans_MaDatSan",
                        column: x => x.MaDatSan,
                        principalTable: "DatSans",
                        principalColumn: "MaDatSan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SuDungDichVus_DichVus_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatSans_MaSan_NgaySuDung_TrangThai",
                table: "DatSans",
                columns: new[] { "MaSan", "NgaySuDung", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_TenDichVu",
                table: "DichVus",
                column: "TenDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaDatSan",
                table: "SuDungDichVus",
                column: "MaDatSan");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaDichVu",
                table: "SuDungDichVus",
                column: "MaDichVu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuDungDichVus");

            migrationBuilder.DropTable(
                name: "DatSans");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "SanTheThaos");
        }
    }
}
