using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseFile.Entities.Migrations
{
    public partial class QuestionAnswerCharsNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CharsNo",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CharsNo",
                table: "Questions");
        }
    }
}
