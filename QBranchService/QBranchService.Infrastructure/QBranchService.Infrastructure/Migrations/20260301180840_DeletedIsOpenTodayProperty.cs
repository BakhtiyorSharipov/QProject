using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QBranchService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeletedIsOpenTodayProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOpenToday",
                table: "BranchConfigurations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOpenToday",
                table: "BranchConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
