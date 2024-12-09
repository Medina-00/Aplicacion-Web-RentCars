using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class migracionEntidades1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehiculo_Categoria_IdCategoria",
                table: "Vehiculo");

            migrationBuilder.DropTable(
                name: "Categoria");

            migrationBuilder.DropIndex(
                name: "IX_Vehiculo_IdCategoria",
                table: "Vehiculo");

            migrationBuilder.DropColumn(
                name: "IdCategoria",
                table: "Vehiculo");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Vehiculo",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Vehiculo");

            migrationBuilder.AddColumn<int>(
                name: "IdCategoria",
                table: "Vehiculo",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categoria",
                columns: table => new
                {
                    IdCategoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameDescripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categoria", x => x.IdCategoria);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vehiculo_IdCategoria",
                table: "Vehiculo",
                column: "IdCategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehiculo_Categoria_IdCategoria",
                table: "Vehiculo",
                column: "IdCategoria",
                principalTable: "Categoria",
                principalColumn: "IdCategoria",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
