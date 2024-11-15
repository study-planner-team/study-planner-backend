using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToStudySessionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TopicTitle",
                table: "StudySessions");

            migrationBuilder.AddColumn<int>(
                name: "TopicId",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_TopicId",
                table: "StudySessions",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_StudyTopics_TopicId",
                table: "StudySessions",
                column: "TopicId",
                principalTable: "StudyTopics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_StudyTopics_TopicId",
                table: "StudySessions");

            migrationBuilder.DropIndex(
                name: "IX_StudySessions_TopicId",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "TopicId",
                table: "StudySessions");

            migrationBuilder.AddColumn<string>(
                name: "TopicTitle",
                table: "StudySessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
