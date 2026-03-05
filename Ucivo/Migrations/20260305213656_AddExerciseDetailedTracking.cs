using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurboSkola.Migrations
{
    /// <inheritdoc />
    public partial class AddExerciseDetailedTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GaveUp",
                table: "SessionExercises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HintShown",
                table: "SessionExercises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WrongAnswers",
                table: "SessionExercises",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GaveUp",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "HintShown",
                table: "SessionExercises");

            migrationBuilder.DropColumn(
                name: "WrongAnswers",
                table: "SessionExercises");
        }
    }
}
