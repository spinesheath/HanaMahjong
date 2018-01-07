using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Spines.Hana.Blame.Data.Migrations
{
    public partial class FlatFrameComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Thread_ThreadId",
                table: "Comment");

            migrationBuilder.DropTable(
                name: "Thread");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ThreadId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Comment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FrameIndex",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameIndex",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeatIndex",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_MatchId",
                table: "Comment",
                column: "MatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Match_MatchId",
                table: "Comment",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Match_MatchId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_MatchId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "FrameIndex",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "GameIndex",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "SeatIndex",
                table: "Comment");

            migrationBuilder.AddColumn<long>(
                name: "ThreadId",
                table: "Comment",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Thread",
                columns: table => new
                {
                    FrameId = table.Column<int>(nullable: true),
                    GameId = table.Column<int>(nullable: true),
                    MatchId = table.Column<int>(nullable: true),
                    ParticipantId = table.Column<int>(nullable: true),
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    Hand = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thread", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thread_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Thread_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Thread_Participant_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ThreadId",
                table: "Comment",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Thread_GameId",
                table: "Thread",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Thread_ParticipantId",
                table: "Thread",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Thread_MatchId_GameId_FrameId_ParticipantId",
                table: "Thread",
                columns: new[] { "MatchId", "GameId", "FrameId", "ParticipantId" },
                unique: true,
                filter: "[MatchId] IS NOT NULL AND [GameId] IS NOT NULL AND [FrameId] IS NOT NULL AND [ParticipantId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Thread_Hand",
                table: "Thread",
                column: "Hand",
                unique: true,
                filter: "[Hand] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Thread_ThreadId",
                table: "Comment",
                column: "ThreadId",
                principalTable: "Thread",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
