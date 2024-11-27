using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StreetService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointToStreetGeometry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Streets",
                type: "bytea",
                rowVersion: true,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "Streets");
        }
    }
}
