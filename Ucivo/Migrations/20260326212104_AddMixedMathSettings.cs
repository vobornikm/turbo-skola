using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurboSkola.Migrations
{
    /// <inheritdoc />
    public partial class AddMixedMathSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MixedMathSettingsJson",
                table: "UserSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrainingTypeCode",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MixedMathSettingsJson",
                table: "UserSettings");

            migrationBuilder.AlterColumn<string>(
                name: "TrainingTypeCode",
                table: "TrainingSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
