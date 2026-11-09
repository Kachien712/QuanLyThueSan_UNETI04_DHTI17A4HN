using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyThueSan_UNETI04_DHTI17A4HN.Migrations
{
    /// <inheritdoc />
    public partial class ThemHinhAnhSanTheThao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HinhAnh",
                table: "SanTheThaos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HinhAnh",
                table: "SanTheThaos");
        }
    }
}
