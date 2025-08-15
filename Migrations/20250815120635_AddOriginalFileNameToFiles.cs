using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonBank.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalFileNameToFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalFileName",
                table: "Files",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalFileName",
                table: "Files");
        }
    }
}
