using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TurboSkola.Migrations
{
    /// <inheritdoc />
    public partial class AddPreparedTrainings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PreparedTrainings",
                columns: table => new
                {
                    PreparedTrainingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedByProfileId = table.Column<int>(type: "int", nullable: false),
                    AssignedToProfileId = table.Column<int>(type: "int", nullable: false),
                    TrainingTypeCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SettingsJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScheduledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BatchId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparedTrainings", x => x.PreparedTrainingId);
                    table.ForeignKey(
                        name: "FK_PreparedTrainings_UserProfiles_CreatedByProfileId",
                        column: x => x.CreatedByProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PreparedTrainings_UserProfiles_AssignedToProfileId",
                        column: x => x.AssignedToProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "ProfileId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PreparedTrainings_AssignedToProfileId_Status_ScheduledDate",
                table: "PreparedTrainings",
                columns: new[] { "AssignedToProfileId", "Status", "ScheduledDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PreparedTrainings_CreatedByProfileId",
                table: "PreparedTrainings",
                column: "CreatedByProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreparedTrainings");
        }
    }
}
