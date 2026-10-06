using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    TenDichVu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DonViTinh = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVus", x => x.MaDichVu);
                });

            migrationBuilder.CreateTable(
                name: "KhuVucs",
                columns: table => new
                {
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhuVuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ViTri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuVucs", x => x.MaKhuVuc);
                });

            migrationBuilder.CreateTable(
                name: "LoaiSans",
                columns: table => new
                {
                    MaLoaiSan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoaiSan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoNguoiToiDa = table.Column<int>(type: "int", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoaiSans", x => x.MaLoaiSan);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoans",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoans", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "SanTheThaos",
                columns: table => new
                {
                    MaSan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenSan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaLoaiSan = table.Column<int>(type: "int", nullable: false),
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false),
                    SucChua = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TinhTrang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanTheThaos", x => x.MaSan);
                    table.ForeignKey(
                        name: "FK_SanTheThaos_KhuVucs_MaKhuVuc",
                        column: x => x.MaKhuVuc,
                        principalTable: "KhuVucs",
                        principalColumn: "MaKhuVuc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SanTheThaos_LoaiSans_MaLoaiSan",
                        column: x => x.MaLoaiSan,
                        principalTable: "LoaiSans",
                        principalColumn: "MaLoaiSan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.MaKhachHang);
                    table.ForeignKey(
                        name: "FK_KhachHangs_TaiKhoans_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoans",
                        principalColumn: "MaTaiKhoan");
                });

            migrationBuilder.CreateTable(
                name: "BangGias",
                columns: table => new
                {
                    MaBangGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSan = table.Column<int>(type: "int", nullable: false),
                    TenBangGia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    DonGiaMoiGio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NgayApDungTu = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayApDungDen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangGias", x => x.MaBangGia);
                    table.ForeignKey(
                        name: "FK_BangGias_SanTheThaos_MaSan",
                        column: x => x.MaSan,
                        principalTable: "SanTheThaos",
                        principalColumn: "MaSan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DatSans",
                columns: table => new
                {
                    MaDatSan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaSan = table.Column<int>(type: "int", nullable: false),
                    NgaySuDung = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioBatDau = table.Column<TimeSpan>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeSpan>(type: "time", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoNguoi = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatSans", x => x.MaDatSan);
                    table.ForeignKey(
                        name: "FK_DatSans_KhachHangs_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHangs",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Restrict);
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
                    DonGia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SuDungDichVus_DichVus_MaDichVu",
                        column: x => x.MaDichVu,
                        principalTable: "DichVus",
                        principalColumn: "MaDichVu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToans",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDatSan = table.Column<int>(type: "int", nullable: false),
                    TienSan = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TienDichVu = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToans", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToans_DatSans_MaDatSan",
                        column: x => x.MaDatSan,
                        principalTable: "DatSans",
                        principalColumn: "MaDatSan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangGias_MaSan",
                table: "BangGias",
                column: "MaSan");

            migrationBuilder.CreateIndex(
                name: "IX_DatSans_MaKhachHang",
                table: "DatSans",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DatSans_MaSan",
                table: "DatSans",
                column: "MaSan");

            migrationBuilder.CreateIndex(
                name: "IX_DichVus_TenDichVu",
                table: "DichVus",
                column: "TenDichVu",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_MaTaiKhoan",
                table: "KhachHangs",
                column: "MaTaiKhoan",
                unique: true,
                filter: "[MaTaiKhoan] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KhuVucs_TenKhuVuc",
                table: "KhuVucs",
                column: "TenKhuVuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoaiSans_TenLoaiSan",
                table: "LoaiSans",
                column: "TenLoaiSan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SanTheThaos_MaKhuVuc",
                table: "SanTheThaos",
                column: "MaKhuVuc");

            migrationBuilder.CreateIndex(
                name: "IX_SanTheThaos_MaLoaiSan",
                table: "SanTheThaos",
                column: "MaLoaiSan");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaDatSan",
                table: "SuDungDichVus",
                column: "MaDatSan");

            migrationBuilder.CreateIndex(
                name: "IX_SuDungDichVus_MaDichVu",
                table: "SuDungDichVus",
                column: "MaDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoans_TenDangNhap",
                table: "TaiKhoans",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToans_MaDatSan",
                table: "ThanhToans",
                column: "MaDatSan",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangGias");

            migrationBuilder.DropTable(
                name: "SuDungDichVus");

            migrationBuilder.DropTable(
                name: "ThanhToans");

            migrationBuilder.DropTable(
                name: "DichVus");

            migrationBuilder.DropTable(
                name: "DatSans");

            migrationBuilder.DropTable(
                name: "KhachHangs");

            migrationBuilder.DropTable(
                name: "SanTheThaos");

            migrationBuilder.DropTable(
                name: "TaiKhoans");

            migrationBuilder.DropTable(
                name: "KhuVucs");

            migrationBuilder.DropTable(
                name: "LoaiSans");
        }
    }
}
