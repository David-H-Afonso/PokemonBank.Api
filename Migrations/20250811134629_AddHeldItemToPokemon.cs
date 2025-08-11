using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonBank.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddHeldItemToPokemon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeldItemId",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeldItemId",
                table: "Pokemon");
        }
    }
}
