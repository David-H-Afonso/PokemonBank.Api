using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonBank.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Sha256 = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    Format = table.Column<string>(type: "TEXT", nullable: false),
                    Size = table.Column<long>(type: "INTEGER", nullable: false),
                    StoredPath = table.Column<string>(type: "TEXT", nullable: false),
                    ImportedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RawBlob = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Moves",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    Slot = table.Column<int>(type: "INTEGER", nullable: false),
                    MoveId = table.Column<int>(type: "INTEGER", nullable: false),
                    PpUps = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moves", x => new { x.PokemonId, x.Slot });
                });

            migrationBuilder.CreateTable(
                name: "Pokemon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpeciesId = table.Column<int>(type: "INTEGER", nullable: false),
                    Nickname = table.Column<string>(type: "TEXT", nullable: true),
                    OtName = table.Column<string>(type: "TEXT", nullable: false),
                    Tid = table.Column<int>(type: "INTEGER", nullable: false),
                    Sid = table.Column<int>(type: "INTEGER", nullable: false),
                    Level = table.Column<int>(type: "INTEGER", nullable: false),
                    IsShiny = table.Column<bool>(type: "INTEGER", nullable: false),
                    Nature = table.Column<int>(type: "INTEGER", nullable: false),
                    AbilityId = table.Column<int>(type: "INTEGER", nullable: false),
                    BallId = table.Column<int>(type: "INTEGER", nullable: false),
                    TeraType = table.Column<int>(type: "INTEGER", nullable: true),
                    OriginGame = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    MetDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MetLocation = table.Column<string>(type: "TEXT", nullable: true),
                    SpriteKey = table.Column<string>(type: "TEXT", nullable: false),
                    Favorite = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PokemonTags",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PokemonTags", x => new { x.PokemonId, x.TagId });
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    PokemonId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IvHp = table.Column<int>(type: "INTEGER", nullable: false),
                    IvAtk = table.Column<int>(type: "INTEGER", nullable: false),
                    IvDef = table.Column<int>(type: "INTEGER", nullable: false),
                    IvSpa = table.Column<int>(type: "INTEGER", nullable: false),
                    IvSpd = table.Column<int>(type: "INTEGER", nullable: false),
                    IvSpe = table.Column<int>(type: "INTEGER", nullable: false),
                    EvHp = table.Column<int>(type: "INTEGER", nullable: false),
                    EvAtk = table.Column<int>(type: "INTEGER", nullable: false),
                    EvDef = table.Column<int>(type: "INTEGER", nullable: false),
                    EvSpa = table.Column<int>(type: "INTEGER", nullable: false),
                    EvSpd = table.Column<int>(type: "INTEGER", nullable: false),
                    EvSpe = table.Column<int>(type: "INTEGER", nullable: false),
                    HyperTrainedHp = table.Column<bool>(type: "INTEGER", nullable: false),
                    HyperTrainedAtk = table.Column<bool>(type: "INTEGER", nullable: false),
                    HyperTrainedDef = table.Column<bool>(type: "INTEGER", nullable: false),
                    HyperTrainedSpa = table.Column<bool>(type: "INTEGER", nullable: false),
                    HyperTrainedSpd = table.Column<bool>(type: "INTEGER", nullable: false),
                    HyperTrainedSpe = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.PokemonId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_Sha256",
                table: "Files",
                column: "Sha256",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_OriginGame",
                table: "Pokemon",
                column: "OriginGame");

            migrationBuilder.CreateIndex(
                name: "IX_Pokemon_SpeciesId_IsShiny",
                table: "Pokemon",
                columns: new[] { "SpeciesId", "IsShiny" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Moves");

            migrationBuilder.DropTable(
                name: "Pokemon");

            migrationBuilder.DropTable(
                name: "PokemonTags");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
