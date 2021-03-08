using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CaseFile.Entities.Migrations
{
    public partial class Report : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ReportId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    ReportType = table.Column<int>(nullable: false),
                    Center = table.Column<string>(nullable: true),
                    Period = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    ActiveCasesLastDayOfPreviousMonth = table.Column<int>(nullable: false),
                    NewCasesCurrentMonth = table.Column<int>(nullable: false),
                    ClosedCasesCurrentMonthFamily = table.Column<int>(nullable: false),
                    ClosedCasesCurrentMonthAssistent = table.Column<int>(nullable: false),
                    ClosedCasesCurrentMonthOtherOrg = table.Column<int>(nullable: false),
                    ActiveCasesLastDayOfCurrentMonth = table.Column<int>(nullable: false),
                    ActiveCasesLastDayOfPreviousMonthUR = table.Column<int>(nullable: false),
                    NewCasesCurrentMonthUR = table.Column<int>(nullable: false),
                    ClosedCasesCurrentMonthFamilyUR = table.Column<int>(nullable: false),
                    ClosedCasesCurrentMonthAssistentUR = table.Column<int>(nullable: false),
                    ClosedCasesCurrentMonthOtherOrgUR = table.Column<int>(nullable: false),
                    ActiveCasesLastDayOfCurrentMonthUR = table.Column<int>(nullable: false),
                    TotalChildrenNo = table.Column<int>(nullable: false),
                    ChildrenLessThanOneNo = table.Column<int>(nullable: false),
                    ChildrenOneToTwoNo = table.Column<int>(nullable: false),
                    ChildrenThreeToSixNo = table.Column<int>(nullable: false),
                    ChildrenSevenToNineNo = table.Column<int>(nullable: false),
                    ChildrenTenToThirteenNo = table.Column<int>(nullable: false),
                    ChildrenFourteenToSeventeenNo = table.Column<int>(nullable: false),
                    ChildrenEighteenOrMoreNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Reports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
