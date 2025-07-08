using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NguyenManhDuc.WebApp.Migrations
{
    /// <inheritdoc />
    public partial class _07072025_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Capacity",
                table: "Products",
                newName: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Products",
                newName: "Capacity");
        }
    }
}
