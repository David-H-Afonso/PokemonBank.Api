using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokemonBank.Api.Migrations
{
    /// <inheritdoc />
    public partial class EnhanceEntitiesWithPK9Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatAtk",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatDef",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatHp",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatHpCurrent",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatSpa",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatSpd",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatSpe",
                table: "Stats",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContestBeauty",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContestCool",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContestCute",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContestSheen",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContestSmart",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContestTough",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentFriendship",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentHandler",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EggLocation",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EggMetDate",
                table: "Pokemon",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "EncryptionConstant",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "Experience",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<bool>(
                name: "FatefulEncounter",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Form",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "FormArgument",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerFriendship",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerGender",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerLanguage",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerMemory",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerMemoryFeeling",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerMemoryIntensity",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HandlingTrainerMemoryVariable",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HandlingTrainerName",
                table: "Pokemon",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "HeightScalar",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsEgg",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OriginalTrainerMemory",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginalTrainerMemoryFeeling",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginalTrainerMemoryIntensity",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OriginalTrainerMemoryVariable",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<uint>(
                name: "PersonalityId",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<int>(
                name: "PokerusDays",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PokerusState",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PokerusStrain",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Scale",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WeightScalar",
                table: "Pokemon",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CurrentPp",
                table: "Moves",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatAtk",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatDef",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatHp",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatHpCurrent",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatSpa",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatSpd",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "StatSpe",
                table: "Stats");

            migrationBuilder.DropColumn(
                name: "ContestBeauty",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "ContestCool",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "ContestCute",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "ContestSheen",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "ContestSmart",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "ContestTough",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "CurrentFriendship",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "CurrentHandler",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "EggLocation",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "EggMetDate",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "EncryptionConstant",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "Experience",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "FatefulEncounter",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "Form",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "FormArgument",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerFriendship",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerGender",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerLanguage",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerMemory",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerMemoryFeeling",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerMemoryIntensity",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerMemoryVariable",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HandlingTrainerName",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "HeightScalar",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "IsEgg",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalTrainerMemory",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalTrainerMemoryFeeling",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalTrainerMemoryIntensity",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "OriginalTrainerMemoryVariable",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "PersonalityId",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "PokerusDays",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "PokerusState",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "PokerusStrain",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "Scale",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "WeightScalar",
                table: "Pokemon");

            migrationBuilder.DropColumn(
                name: "CurrentPp",
                table: "Moves");
        }
    }
}
