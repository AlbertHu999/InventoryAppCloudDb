using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace InventoryAppCloudDb.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryLedgerAndVoidFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "user_tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "revoked_at",
                table: "user_tokens",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "sales_orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Posted");

            migrationBuilder.AddColumn<string>(
                name: "void_reason",
                table: "sales_orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "voided_at",
                table: "sales_orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "voided_by",
                table: "sales_orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "purchase_orders",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Posted");

            migrationBuilder.AddColumn<string>(
                name: "void_reason",
                table: "purchase_orders",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "voided_at",
                table: "purchase_orders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "voided_by",
                table: "purchase_orders",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "products",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "inventory_ledgers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    product_id = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    source_order_id = table.Column<int>(type: "integer", nullable: false),
                    source_detail_id = table.Column<int>(type: "integer", nullable: true),
                    direction = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    remark = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_ledgers", x => x.id);
                    table.ForeignKey(
                        name: "FK_inventory_ledgers_products_product_id",
                        column: x => x.product_id,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_inventory_ledgers_product_id",
                table: "inventory_ledgers",
                column: "product_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "inventory_ledgers");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "user_tokens");

            migrationBuilder.DropColumn(
                name: "revoked_at",
                table: "user_tokens");

            migrationBuilder.DropColumn(
                name: "status",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "void_reason",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "voided_at",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "voided_by",
                table: "sales_orders");

            migrationBuilder.DropColumn(
                name: "status",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "void_reason",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "voided_at",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "voided_by",
                table: "purchase_orders");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "products");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "products");
        }
    }
}
