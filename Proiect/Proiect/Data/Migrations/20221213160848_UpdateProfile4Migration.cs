using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class UpdateProfile4Migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Visible",
                table: "Profiles");

            migrationBuilder.AddColumn<string>(
                name: "Privacy",
                table: "Profiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "Profiles");

            migrationBuilder.AddColumn<bool>(
                name: "Visible",
                table: "Profiles",
                type: "bit",
                nullable: true);
        }
    }
}
