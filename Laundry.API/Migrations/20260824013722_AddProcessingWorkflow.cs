using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Laundry.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProcessingWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProcessingWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceCategoryId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingWorkflows_ServiceCategories_ServiceCategoryId",
                        column: x => x.ServiceCategoryId,
                        principalTable: "ServiceCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemProcessings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemId = table.Column<int>(type: "integer", nullable: false),
                    ProcessingWorkflowId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemProcessings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemProcessings_OrderItems_OrderItemId",
                        column: x => x.OrderItemId,
                        principalTable: "OrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemProcessings_ProcessingWorkflows_ProcessingWorkflow~",
                        column: x => x.ProcessingWorkflowId,
                        principalTable: "ProcessingWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessingWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProcessingWorkflowId = table.Column<int>(type: "integer", nullable: false),
                    StepType = table.Column<int>(type: "integer", nullable: false),
                    Sequence = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessingWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessingWorkflowSteps_ProcessingWorkflows_ProcessingWorkf~",
                        column: x => x.ProcessingWorkflowId,
                        principalTable: "ProcessingWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemProcessingSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderItemProcessingId = table.Column<int>(type: "integer", nullable: false),
                    ProcessingWorkflowStepId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CompletedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemProcessingSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItemProcessingSteps_OrderItemProcessings_OrderItemProc~",
                        column: x => x.OrderItemProcessingId,
                        principalTable: "OrderItemProcessings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItemProcessingSteps_ProcessingWorkflowSteps_Processing~",
                        column: x => x.ProcessingWorkflowStepId,
                        principalTable: "ProcessingWorkflowSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemProcessings_OrderItemId",
                table: "OrderItemProcessings",
                column: "OrderItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemProcessings_ProcessingWorkflowId",
                table: "OrderItemProcessings",
                column: "ProcessingWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemProcessings_Status",
                table: "OrderItemProcessings",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemProcessingSteps_OrderItemProcessingId_ProcessingWo~",
                table: "OrderItemProcessingSteps",
                columns: new[] { "OrderItemProcessingId", "ProcessingWorkflowStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemProcessingSteps_ProcessingWorkflowStepId",
                table: "OrderItemProcessingSteps",
                column: "ProcessingWorkflowStepId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemProcessingSteps_Status",
                table: "OrderItemProcessingSteps",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingWorkflows_ServiceCategoryId",
                table: "ProcessingWorkflows",
                column: "ServiceCategoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessingWorkflowSteps_ProcessingWorkflowId_Sequence",
                table: "ProcessingWorkflowSteps",
                columns: new[] { "ProcessingWorkflowId", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemProcessingSteps");

            migrationBuilder.DropTable(
                name: "OrderItemProcessings");

            migrationBuilder.DropTable(
                name: "ProcessingWorkflowSteps");

            migrationBuilder.DropTable(
                name: "ProcessingWorkflows");
        }
    }
}
