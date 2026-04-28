using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeeEcommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductRatingImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId",
                schema: "Core",
                table: "ProductImages");

            migrationBuilder.AddColumn<string>(
                name: "SKU",
                schema: "Core",
                table: "Products",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                schema: "Core",
                table: "ProductImages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                schema: "Core",
                table: "ProductImages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                schema: "Core",
                table: "ProductImages",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                schema: "Core",
                table: "Products",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                schema: "Core",
                table: "Products",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                schema: "Core",
                table: "ProductImages",
                column: "ProductId",
                unique: true,
                filter: "[IsMain] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId_DisplayOrder",
                schema: "Core",
                table: "ProductImages",
                columns: new[] { "ProductId", "DisplayOrder" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Name",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SKU",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId",
                schema: "Core",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ProductId_DisplayOrder",
                schema: "Core",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "SKU",
                schema: "Core",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AltText",
                schema: "Core",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                schema: "Core",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                schema: "Core",
                table: "ProductImages");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                schema: "Core",
                table: "ProductImages",
                column: "ProductId");
        }
    }
}
