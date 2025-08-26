using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPrioridadReclamo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "Prioridad",
                table: "Reclamos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Prioridad",
                table: "Reclamos");
        }
    }
}
