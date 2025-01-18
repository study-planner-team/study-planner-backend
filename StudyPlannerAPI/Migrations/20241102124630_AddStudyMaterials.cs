using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyPlannerAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStudyMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StudyMaterials",
                columns: table => new
                {
                    StudyMaterialId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudyTopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyMaterials", x => x.StudyMaterialId);
                    table.ForeignKey(
                        name: "FK_StudyMaterials_StudyTopics_StudyTopicId",
                        column: x => x.StudyTopicId,
                        principalTable: "StudyTopics",
                        principalColumn: "TopicId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudyMaterials_StudyTopicId",
                table: "StudyMaterials",
                column: "StudyTopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyMaterials");
        }
    }
}
