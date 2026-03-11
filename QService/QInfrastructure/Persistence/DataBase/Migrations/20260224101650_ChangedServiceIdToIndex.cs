using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QInfrastructure.Persistence.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class ChangedServiceIdToIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queues_Services_ServiceId",
                table: "Queues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Queues_Services_ServiceId",
                table: "Queues",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
