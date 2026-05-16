using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeeEcommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductRatings_Users_UserId",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "Core",
                table: "ProductRatings",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "Stars",
                schema: "Core",
                table: "ProductRatings",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_ProductRatings_UserId",
                schema: "Core",
                table: "ProductRatings",
                newName: "IX_ProductRatings_CreatorId");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "Core",
                table: "ProductRatings",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                schema: "Core",
                table: "ProductRatings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HelpfulCount",
                schema: "Core",
                table: "ProductRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderItemId",
                schema: "Core",
                table: "ProductRatings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                schema: "Core",
                table: "ProductRatings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "Core",
                table: "ProductRatings",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductRatingSummaries",
                schema: "Core",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalCount = table.Column<int>(type: "int", nullable: false),
                    TotalSum = table.Column<decimal>(type: "decimal(9,1)", precision: 9, scale: 1, nullable: false),
                    Count1Star = table.Column<int>(type: "int", nullable: false),
                    Count2Star = table.Column<int>(type: "int", nullable: false),
                    Count3Star = table.Column<int>(type: "int", nullable: false),
                    Count4Star = table.Column<int>(type: "int", nullable: false),
                    Count5Star = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRatingSummaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRatingSummaries_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Core",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRatings_OrderItemId",
                schema: "Core",
                table: "ProductRatings",
                column: "OrderItemId",
                unique: true,
                filter: "[OrderItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRatings_ProductId_CreatorId",
                schema: "Core",
                table: "ProductRatings",
                columns: new[] { "ProductId", "CreatorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductRatingSummaries_ProductId",
                schema: "Core",
                table: "ProductRatingSummaries",
                column: "ProductId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRatings_OrderItems_OrderItemId",
                schema: "Core",
                table: "ProductRatings",
                column: "OrderItemId",
                principalSchema: "Core",
                principalTable: "OrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRatings_Users_CreatorId",
                schema: "Core",
                table: "ProductRatings",
                column: "CreatorId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductRatings_OrderItems_OrderItemId",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductRatings_Users_CreatorId",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropTable(
                name: "ProductRatingSummaries",
                schema: "Core");

            migrationBuilder.DropIndex(
                name: "IX_ProductRatings_OrderItemId",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropIndex(
                name: "IX_ProductRatings_ProductId_CreatorId",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "HelpfulCount",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "OrderItemId",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "Core",
                table: "ProductRatings");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "Core",
                table: "ProductRatings",
                newName: "Stars");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "Core",
                table: "ProductRatings",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductRatings_CreatorId",
                schema: "Core",
                table: "ProductRatings",
                newName: "IX_ProductRatings_UserId");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                schema: "Core",
                table: "ProductRatings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRatings_Users_UserId",
                schema: "Core",
                table: "ProductRatings",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
