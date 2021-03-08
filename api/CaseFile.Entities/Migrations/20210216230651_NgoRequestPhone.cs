using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseFile.Entities.Migrations
{
    public partial class NgoRequestPhone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "NgoRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "NgoRequests");
        }
    }
}
