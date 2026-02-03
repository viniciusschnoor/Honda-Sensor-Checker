using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HondaSensorChecker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    OperatorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Re = table.Column<string>(type: "TEXT", nullable: false),
                    ZfId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Admin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => x.OperatorId);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Prefix = table.Column<string>(type: "TEXT", nullable: false),
                    StartPartNumber = table.Column<string>(type: "TEXT", nullable: false),
                    EndPartNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    OperatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_Logs_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "OperatorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SapWorkOrders",
                columns: table => new
                {
                    SapWorkOrderId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkOrderNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SapWorkOrders", x => x.SapWorkOrderId);
                    table.ForeignKey(
                        name: "FK_SapWorkOrders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierBoxes",
                columns: table => new
                {
                    SupplierBoxId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UniqueNumber = table.Column<string>(type: "TEXT", nullable: false),
                    QtySupplied = table.Column<int>(type: "INTEGER", nullable: false),
                    QtyRemaining = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierBoxes", x => x.SupplierBoxId);
                    table.ForeignKey(
                        name: "FK_SupplierBoxes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZfBoxes",
                columns: table => new
                {
                    ZfBoxId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QtyToSend = table.Column<int>(type: "INTEGER", nullable: false),
                    UniqueNumber = table.Column<string>(type: "TEXT", nullable: true),
                    Batch = table.Column<string>(type: "TEXT", nullable: true),
                    InProgress = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    SapWorkOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    OperatorId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZfBoxes", x => x.ZfBoxId);
                    table.ForeignKey(
                        name: "FK_ZfBoxes_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "OperatorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZfBoxes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZfBoxes_SapWorkOrders_SapWorkOrderId",
                        column: x => x.SapWorkOrderId,
                        principalTable: "SapWorkOrders",
                        principalColumn: "SapWorkOrderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    SensorId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ScannedTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InProgress = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    OperatorId = table.Column<int>(type: "INTEGER", nullable: false),
                    SupplierBoxId = table.Column<int>(type: "INTEGER", nullable: false),
                    SapWorkOrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    ZfBoxId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.SensorId);
                    table.ForeignKey(
                        name: "FK_Sensors_Operators_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Operators",
                        principalColumn: "OperatorId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sensors_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sensors_SapWorkOrders_SapWorkOrderId",
                        column: x => x.SapWorkOrderId,
                        principalTable: "SapWorkOrders",
                        principalColumn: "SapWorkOrderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sensors_SupplierBoxes_SupplierBoxId",
                        column: x => x.SupplierBoxId,
                        principalTable: "SupplierBoxes",
                        principalColumn: "SupplierBoxId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sensors_ZfBoxes_ZfBoxId",
                        column: x => x.ZfBoxId,
                        principalTable: "ZfBoxes",
                        principalColumn: "ZfBoxId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperatorId",
                table: "Logs",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SapWorkOrders_ProductId",
                table: "SapWorkOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_OperatorId",
                table: "Sensors",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_ProductId",
                table: "Sensors",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_SapWorkOrderId",
                table: "Sensors",
                column: "SapWorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_SupplierBoxId",
                table: "Sensors",
                column: "SupplierBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_ZfBoxId",
                table: "Sensors",
                column: "ZfBoxId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierBoxes_ProductId",
                table: "SupplierBoxes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ZfBoxes_OperatorId",
                table: "ZfBoxes",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_ZfBoxes_ProductId",
                table: "ZfBoxes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ZfBoxes_SapWorkOrderId",
                table: "ZfBoxes",
                column: "SapWorkOrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "SupplierBoxes");

            migrationBuilder.DropTable(
                name: "ZfBoxes");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "SapWorkOrders");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
