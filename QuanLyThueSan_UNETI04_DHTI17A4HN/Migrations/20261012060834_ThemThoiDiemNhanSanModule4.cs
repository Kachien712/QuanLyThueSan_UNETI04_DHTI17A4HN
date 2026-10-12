using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemThoiDiemNhanSanModule4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiDiemNhanSanThucTe",
                table: "DatSans",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThoiDiemNhanSanThucTe",
                table: "DatSans");
        }
    }
}
