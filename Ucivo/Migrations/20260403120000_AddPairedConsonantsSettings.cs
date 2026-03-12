using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurboSkola.Migrations
{
    /// <inheritdoc />
    public partial class AddPairedConsonantsSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PairedConsonantsSettingsJson",
                table: "UserSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PairedConsonantsSettingsJson",
                table: "UserSettings");
        }
    }
}
