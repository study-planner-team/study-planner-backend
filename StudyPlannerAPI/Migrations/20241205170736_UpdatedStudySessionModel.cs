using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStudySessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ActualDuration",
                table: "StudySessions",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "StudySessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WasMissed",
                table: "StudySessions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDuration",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "WasMissed",
                table: "StudySessions");
        }
    }
}
