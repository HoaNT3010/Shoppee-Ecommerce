using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppeeEcommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class User_Avatar_Trackable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarPublicId",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarUrl",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "Identity",
                table: "Users",
                type: "datetime2",
                nullable: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                schema: "Identity",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarPublicId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AvatarUrl",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                schema: "Identity",
                table: "Users");
        }
    }
}
