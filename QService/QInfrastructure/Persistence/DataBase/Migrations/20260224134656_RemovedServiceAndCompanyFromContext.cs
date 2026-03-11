using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QInfrastructure.Persistence.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RemovedServiceAndCompanyFromContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Services_ServiceId",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "ServiceEntityId",
                table: "Employees",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ServiceEntityId",
                table: "Employees",
                column: "ServiceEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Services_ServiceEntityId",
                table: "Employees",
                column: "ServiceEntityId",
                principalTable: "Services",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Services_ServiceEntityId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ServiceEntityId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ServiceEntityId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Services_ServiceId",
                table: "Employees",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id");
        }
    }
}
