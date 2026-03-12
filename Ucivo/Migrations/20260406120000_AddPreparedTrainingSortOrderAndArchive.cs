using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurboSkola.Migrations
{
    /// <inheritdoc />
    public partial class AddPreparedTrainingSortOrderAndArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "PreparedTrainings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "PreparedTrainings");
        }
    }
}
