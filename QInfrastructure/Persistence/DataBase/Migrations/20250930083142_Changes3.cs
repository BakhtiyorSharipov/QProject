using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QInfrastructure.Persistence.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class Changes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySchedule_EmployeeId",
                table: "AvailabilitySchedule");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySchedule_EmployeeId",
                table: "AvailabilitySchedule",
                column: "EmployeeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AvailabilitySchedule_EmployeeId",
                table: "AvailabilitySchedule");

            migrationBuilder.CreateIndex(
                name: "IX_AvailabilitySchedule_EmployeeId",
                table: "AvailabilitySchedule",
                column: "EmployeeId");
        }
    }
}
