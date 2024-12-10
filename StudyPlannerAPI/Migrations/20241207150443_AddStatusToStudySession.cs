using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToStudySession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "WasMissed",
                table: "StudySessions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudySessions");

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
    }
}
