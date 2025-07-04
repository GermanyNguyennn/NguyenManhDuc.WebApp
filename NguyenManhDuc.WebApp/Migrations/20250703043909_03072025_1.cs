using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NguyenManhDuc.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class _03072025_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NumberOfRAMSlots",
                table: "ProductDetailLaptops",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfRAMSlots",
                table: "ProductDetailLaptops");
        }
    }
}
