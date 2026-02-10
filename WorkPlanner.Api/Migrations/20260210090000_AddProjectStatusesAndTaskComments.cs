using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkPlanner.Api.Migrations;

public partial class AddProjectStatusesAndTaskComments : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "EnabledStatuses",
            table: "Projects",
            type: "TEXT",
            maxLength: 4000,
            nullable: false,
            defaultValue: "Todo,InProgress,Done");

        migrationBuilder.CreateTable(
            name: "TaskComments",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                TaskItemId = table.Column<int>(type: "INTEGER", nullable: false),
                AuthorId = table.Column<string>(type: "TEXT", nullable: false),
                Body = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaskComments", x => x.Id);
                table.ForeignKey(
                    name: "FK_TaskComments_AspNetUsers_AuthorId",
                    column: x => x.AuthorId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_TaskComments_TaskItems_TaskItemId",
                    column: x => x.TaskItemId,
                    principalTable: "TaskItems",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_TaskComments_AuthorId",
            table: "TaskComments",
            column: "AuthorId");

        migrationBuilder.CreateIndex(
            name: "IX_TaskComments_TaskItemId",
            table: "TaskComments",
            column: "TaskItemId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TaskComments");

        migrationBuilder.DropColumn(
            name: "EnabledStatuses",
            table: "Projects");
    }
}
