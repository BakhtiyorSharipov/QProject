using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using QDomain.Models;

#nullable disable

namespace QInfrastructure.Persistence.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Changes4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySchedule_Employees_EmployeeId",
                table: "AvailabilitySchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AvailabilitySchedule",
                table: "AvailabilitySchedule");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySchedule_EmployeeId",
                table: "AvailabilitySchedule");

            migrationBuilder.DropColumn(
                name: "From",
                table: "AvailabilitySchedule");

            migrationBuilder.DropColumn(
                name: "To",
                table: "AvailabilitySchedule");

            migrationBuilder.RenameTable(
                name: "AvailabilitySchedule",
                newName: "AvailabilitySchedules");

            migrationBuilder.AddColumn<List<Interval<DateTimeOffset>>>(
                name: "AvailableSlots",
                table: "AvailabilitySchedules",
                type: "jsonb",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AvailabilitySchedules",
                table: "AvailabilitySchedules",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySchedules_EmployeeId",
                table: "AvailabilitySchedules",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySchedules_Employees_EmployeeId",
                table: "AvailabilitySchedules",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AvailabilitySchedules_Employees_EmployeeId",
                table: "AvailabilitySchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AvailabilitySchedules",
                table: "AvailabilitySchedules");

            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySchedules_EmployeeId",
                table: "AvailabilitySchedules");

            migrationBuilder.DropColumn(
                name: "AvailableSlots",
                table: "AvailabilitySchedules");

            migrationBuilder.RenameTable(
                name: "AvailabilitySchedules",
                newName: "AvailabilitySchedule");

            migrationBuilder.AddColumn<DateTime>(
                name: "From",
                table: "AvailabilitySchedule",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "To",
                table: "AvailabilitySchedule",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_AvailabilitySchedule",
                table: "AvailabilitySchedule",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySchedule_EmployeeId",
                table: "AvailabilitySchedule",
                column: "EmployeeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AvailabilitySchedule_Employees_EmployeeId",
                table: "AvailabilitySchedule",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
