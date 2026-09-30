using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare.API.Migrations
{
    /// <inheritdoc />
    public partial class HealthEventsV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentVisitId",
                table: "HealthEvents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HealthEvents_ParentVisitId",
                table: "HealthEvents",
                column: "ParentVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_HealthEvents_HealthEvents_ParentVisitId",
                table: "HealthEvents",
                column: "ParentVisitId",
                principalTable: "HealthEvents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HealthEvents_HealthEvents_ParentVisitId",
                table: "HealthEvents");

            migrationBuilder.DropIndex(
                name: "IX_HealthEvents_ParentVisitId",
                table: "HealthEvents");

            migrationBuilder.DropColumn(
                name: "ParentVisitId",
                table: "HealthEvents");
        }
    }
}
