using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Spines.Hana.Blame.Data.Migrations
{
    public partial class AddRuleSetTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RuleSetId",
                table: "Match",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RuleSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Aka = table.Column<bool>(type: "bit", nullable: false),
                    ExtraSecondsPerGame = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Kuitan = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PlayerCount = table.Column<int>(type: "int", nullable: false),
                    Rounds = table.Column<int>(type: "int", nullable: false),
                    SecondsPerAction = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleSet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_RuleSetId",
                table: "Match",
                column: "RuleSetId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleSet_Name",
                table: "RuleSet",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Match_RuleSet_RuleSetId",
                table: "Match",
                column: "RuleSetId",
                principalTable: "RuleSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Match_RuleSet_RuleSetId",
                table: "Match");

            migrationBuilder.DropTable(
                name: "RuleSet");

            migrationBuilder.DropIndex(
                name: "IX_Match_RuleSetId",
                table: "Match");

            migrationBuilder.DropColumn(
                name: "RuleSetId",
                table: "Match");
        }
    }
}
