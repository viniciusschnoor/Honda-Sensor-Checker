using HondaSensorChecker.Data.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HondaSensorChecker.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20260818180000_AddPersistentProcessState")]
    public partial class AddPersistentProcessState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentSupplierBoxId",
                table: "ZfBoxes",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "ZfBoxes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(
                """
                UPDATE ZfBoxes
                SET CurrentSupplierBoxId = (
                    SELECT Sensors.SupplierBoxId
                    FROM Sensors
                    WHERE Sensors.ZfBoxId = ZfBoxes.ZfBoxId
                    ORDER BY Sensors.ScannedTime DESC
                    LIMIT 1
                )
                WHERE InProgress = 1;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSupplierBoxId",
                table: "ZfBoxes");

            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "ZfBoxes");
        }
    }
}
