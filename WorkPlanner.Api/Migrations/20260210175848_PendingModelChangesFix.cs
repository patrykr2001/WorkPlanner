using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkPlanner.Api.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelChangesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EnabledStatuses",
                table: "Projects",
                type: "TEXT",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 4000,
                oldDefaultValue: "Todo,InProgress,Done");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EnabledStatuses",
                table: "Projects",
                type: "TEXT",
                maxLength: 4000,
                nullable: false,
                defaultValue: "Todo,InProgress,Done",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 4000);
        }
    }
}
