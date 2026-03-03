using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QBranchService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertiesToBranchConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxTickets",
                table: "BranchConfigurations",
                newName: "MaxTicketsPerDay");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "BreakEndTime",
                table: "BranchConfigurations",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "BreakStartTime",
                table: "BranchConfigurations",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpenToday",
                table: "BranchConfigurations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreakEndTime",
                table: "BranchConfigurations");

            migrationBuilder.DropColumn(
                name: "BreakStartTime",
                table: "BranchConfigurations");

            migrationBuilder.DropColumn(
                name: "IsOpenToday",
                table: "BranchConfigurations");

            migrationBuilder.RenameColumn(
                name: "MaxTicketsPerDay",
                table: "BranchConfigurations",
                newName: "MaxTickets");
        }
    }
}
