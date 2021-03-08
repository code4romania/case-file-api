using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseFile.Entities.Migrations
{
    public partial class NgoRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NgoRequests",
                columns: table => new
                {
                    NgoRequestId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContactPerson = table.Column<string>(nullable: true),
                    NgoName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    RequestDate = table.Column<DateTime>(nullable: false),
                    StatusUpdateDate = table.Column<DateTime>(nullable: true),
                    RequestStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NgoRequests", x => x.NgoRequestId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NgoRequests");
        }
    }
}
