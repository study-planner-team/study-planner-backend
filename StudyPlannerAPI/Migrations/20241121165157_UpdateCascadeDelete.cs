using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_StudyPlans_StudyPlanId",
                table: "StudySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_StudyTopics_TopicId",
                table: "StudySessions");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_StudyPlans_StudyPlanId",
                table: "StudySessions",
                column: "StudyPlanId",
                principalTable: "StudyPlans",
                principalColumn: "StudyPlanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_StudyTopics_TopicId",
                table: "StudySessions",
                column: "TopicId",
                principalTable: "StudyTopics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_StudyPlans_StudyPlanId",
                table: "StudySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_StudyTopics_TopicId",
                table: "StudySessions");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_StudyPlans_StudyPlanId",
                table: "StudySessions",
                column: "StudyPlanId",
                principalTable: "StudyPlans",
                principalColumn: "StudyPlanId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_StudyTopics_TopicId",
                table: "StudySessions",
                column: "TopicId",
                principalTable: "StudyTopics",
                principalColumn: "TopicId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
