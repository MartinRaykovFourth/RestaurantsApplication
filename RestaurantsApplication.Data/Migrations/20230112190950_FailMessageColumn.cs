using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantsApplication.Data.Migrations
{
    public partial class FailMessageColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FailMessage",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FailMessage",
                table: "Requests");
        }
    }
}
