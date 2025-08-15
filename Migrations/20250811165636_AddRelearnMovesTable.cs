using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeastVault.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRelearnMovesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RelearnMoves",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Slot = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelearnMoves", x => new { x.PokemonId, x.Slot });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RelearnMoves");
        }
    }
}
