using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseFile.Entities.Migrations
{
    public partial class UserTemporaryToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TemporaryToken",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TemporaryToken",
                table: "Users");
        }
    }
}
