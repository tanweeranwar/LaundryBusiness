using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Laundry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchPricing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BranchPricings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BranchId = table.Column<int>(type: "integer", nullable: false),
                    ServiceCategoryId = table.Column<int>(type: "integer", nullable: false),
                    GarmentTypeId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    IsExpressAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    ExpressPrice = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    EstimatedProcessingHours = table.Column<int>(type: "integer", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchPricings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchPricings_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchPricings_GarmentTypes_GarmentTypeId",
                        column: x => x.GarmentTypeId,
                        principalTable: "GarmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BranchPricings_ServiceCategories_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchPricings_BranchId_ServiceCategoryId_GarmentTypeId",
                table: "BranchPricings",
                columns: new[] { "BranchId", "ServiceCategoryId", "GarmentTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BranchPricings_GarmentTypeId",
                table: "BranchPricings",
                column: "GarmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchPricings_ServiceCategoryId",
                table: "BranchPricings",
                column: "ServiceCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchPricings");
        }
    }
}
