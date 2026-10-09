using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PetCare.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultEventTypesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTypeDefinitions",
                columns: new[] { "Id", "Color", "CreatedAt", "Icon", "Name", "UserId", "UserId1" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "blue", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "local_hospital", "Visita Veterinaria", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "orange", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "medication", "Medicación", null, null },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "red", new DateTime(2020, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "vaccines", "Vacunación", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypeDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "EventTypeDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "EventTypeDefinitions",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000003"));
        }
    }
}
