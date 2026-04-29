using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeeEcommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Core",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Core",
                table: "Products");
        }
    }
}
