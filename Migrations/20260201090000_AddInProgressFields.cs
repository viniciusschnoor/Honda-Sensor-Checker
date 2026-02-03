using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HondaSensorChecker.Migrations
{
    /// <inheritdoc />
    public partial class AddInProgressFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InProgress",
                table: "ZfBoxes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InProgress",
                table: "Sensors",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Batch",
                table: "ZfBoxes",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Batch",
                table: "ZfBoxes",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "InProgress",
                table: "ZfBoxes");

            migrationBuilder.DropColumn(
                name: "InProgress",
                table: "Sensors");
        }
    }
}
