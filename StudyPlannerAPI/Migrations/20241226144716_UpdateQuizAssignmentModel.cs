using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateQuizAssignmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "QuizAssignments");

            migrationBuilder.AddColumn<int>(
                name: "CorrectAnswers",
                table: "QuizAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuestions",
                table: "QuizAssignments",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswers",
                table: "QuizAssignments");

            migrationBuilder.DropColumn(
                name: "TotalQuestions",
                table: "QuizAssignments");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "QuizAssignments",
                type: "float",
                nullable: true);
        }
    }
}
