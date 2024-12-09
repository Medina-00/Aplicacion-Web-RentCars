using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MigrationEntidades2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Disponible",
                table: "Vehiculo");

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Vehiculo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Vehiculo");

            migrationBuilder.AddColumn<bool>(
                name: "Disponible",
                table: "Vehiculo",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
