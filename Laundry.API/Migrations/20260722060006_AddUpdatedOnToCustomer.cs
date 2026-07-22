using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Laundry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdatedOnToCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Customers");
        }
    }
}
