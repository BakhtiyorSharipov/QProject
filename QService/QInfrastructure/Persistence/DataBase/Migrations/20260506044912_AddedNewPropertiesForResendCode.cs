using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QInfrastructure.Persistence.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewPropertiesForResendCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastCodeSentAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ResendCount",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCodeSentAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResendCount",
                table: "Users");
        }
    }
}
