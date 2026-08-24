using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Laundry.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedProcessingWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ProcessingWorkflows",
                columns: new[] { "Id", "CreatedOn", "IsActive", "Name", "ServiceCategoryId", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Wash", 2, null },
                    { 2, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Dry Clean", 3, null },
                    { 3, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Iron", 4, null }
                });

            migrationBuilder.InsertData(
                table: "ProcessingWorkflowSteps",
                columns: new[] { "Id", "CreatedOn", "IsActive", "IsRequired", "ProcessingWorkflowId", "Sequence", "StepType", "UpdatedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, 1, 1, 1, null },
                    { 2, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, 1, 2, 5, null },
                    { 3, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, 2, 1, 3, null },
                    { 4, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, 2, 2, 5, null },
                    { 5, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, 3, 1, 4, null },
                    { 6, new DateTime(2026, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), true, true, 3, 2, 5, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ProcessingWorkflowSteps",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflowSteps",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflowSteps",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflowSteps",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflowSteps",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflowSteps",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflows",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflows",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProcessingWorkflows",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
