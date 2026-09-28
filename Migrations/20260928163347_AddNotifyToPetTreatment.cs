using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifyToPetTreatment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Notify",
                table: "PetTreatment",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notify",
                table: "PetTreatment");
        }
    }
}
