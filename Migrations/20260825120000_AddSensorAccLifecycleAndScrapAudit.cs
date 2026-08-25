using HondaSensorChecker.Data.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HondaSensorChecker.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20260825120000_AddSensorAccLifecycleAndScrapAudit")]
    public partial class AddSensorAccLifecycleAndScrapAudit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AccCycleId",
                table: "Sensors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccPartTypeId",
                table: "Sensors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccState",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<DateTime>(
                name: "AccUnloadTime",
                table: "Sensors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccUnloadOtherInfo",
                table: "Sensors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccUnitPartTypeId",
                table: "Sensors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsScrap",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ScrapOperatorId",
                table: "Sensors",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ScrapOperatorName",
                table: "Sensors",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScrappedTime",
                table: "Sensors",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "AccCycleId", table: "Sensors");
            migrationBuilder.DropColumn(name: "AccPartTypeId", table: "Sensors");
            migrationBuilder.DropColumn(name: "AccState", table: "Sensors");
            migrationBuilder.DropColumn(name: "AccUnloadTime", table: "Sensors");
            migrationBuilder.DropColumn(name: "AccUnloadOtherInfo", table: "Sensors");
            migrationBuilder.DropColumn(name: "AccUnitPartTypeId", table: "Sensors");
            migrationBuilder.DropColumn(name: "IsScrap", table: "Sensors");
            migrationBuilder.DropColumn(name: "ScrapOperatorId", table: "Sensors");
            migrationBuilder.DropColumn(name: "ScrapOperatorName", table: "Sensors");
            migrationBuilder.DropColumn(name: "ScrappedTime", table: "Sensors");
        }
    }
}
