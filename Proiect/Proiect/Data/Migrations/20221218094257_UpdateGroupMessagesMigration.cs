using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proiect.Data.Migrations
{
    public partial class UpdateGroupMessagesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Message_AspNetUsers_CreatorId",
                table: "Group_Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_Message_Groups_GroupId",
                table: "Group_Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group_Message",
                table: "Group_Message");

            migrationBuilder.RenameTable(
                name: "Group_Message",
                newName: "Group_Messages");

            migrationBuilder.RenameIndex(
                name: "IX_Group_Message_GroupId",
                table: "Group_Messages",
                newName: "IX_Group_Messages_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Group_Message_CreatorId",
                table: "Group_Messages",
                newName: "IX_Group_Messages_CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Group_Messages",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group_Messages",
                table: "Group_Messages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Messages_AspNetUsers_CreatorId",
                table: "Group_Messages",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Messages_Groups_GroupId",
                table: "Group_Messages",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Messages_AspNetUsers_CreatorId",
                table: "Group_Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_Messages_Groups_GroupId",
                table: "Group_Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group_Messages",
                table: "Group_Messages");

            migrationBuilder.RenameTable(
                name: "Group_Messages",
                newName: "Group_Message");

            migrationBuilder.RenameIndex(
                name: "IX_Group_Messages_GroupId",
                table: "Group_Message",
                newName: "IX_Group_Message_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Group_Messages_CreatorId",
                table: "Group_Message",
                newName: "IX_Group_Message_CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Group_Message",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group_Message",
                table: "Group_Message",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Message_AspNetUsers_CreatorId",
                table: "Group_Message",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Message_Groups_GroupId",
                table: "Group_Message",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
