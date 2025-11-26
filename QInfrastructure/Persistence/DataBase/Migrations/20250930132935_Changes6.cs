using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using QDomain.Models;

#nullable disable

namespace QInfrastructure.Persistence.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Changes6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Interval<DateTimeOffset>>>(
                name: "AvailableSlots",
                table: "AvailabilitySchedules",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(List<Interval<DateTimeOffset>>),
                oldType: "jsonb",
                oldDefaultValueSql: " '[]'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<Interval<DateTimeOffset>>>(
                name: "AvailableSlots",
                table: "AvailabilitySchedules",
                type: "jsonb",
                nullable: false,
                defaultValueSql: " '[]'::jsonb",
                oldClrType: typeof(List<Interval<DateTimeOffset>>),
                oldType: "jsonb");
        }
    }
}
