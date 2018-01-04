using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Spines.Hana.Blame.Data.Migrations
{
    public partial class AddFrameThread : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Hand",
                table: "Thread",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrameId",
                table: "Thread",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Thread",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MatchId",
                table: "Thread",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantId",
                table: "Thread",
                type: "int",
                nullable: true);

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
                name: "FK_Thread_Game_GameId",
                table: "Thread",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Thread_Match_MatchId",
                table: "Thread",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Thread_Participant_ParticipantId",
                table: "Thread",
                column: "ParticipantId",
                principalTable: "Participant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Game_GameId",
                table: "Thread");

            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Match_MatchId",
                table: "Thread");

            migrationBuilder.DropForeignKey(
                name: "FK_Thread_Participant_ParticipantId",
                table: "Thread");

            migrationBuilder.DropIndex(
                name: "IX_Thread_GameId",
                table: "Thread");

            migrationBuilder.DropIndex(
                name: "IX_Thread_ParticipantId",
                table: "Thread");

            migrationBuilder.DropIndex(
                name: "IX_Thread_MatchId_GameId_FrameId_ParticipantId",
                table: "Thread");

            migrationBuilder.DropIndex(
                name: "IX_Thread_Hand",
                table: "Thread");

            migrationBuilder.DropColumn(
                name: "FrameId",
                table: "Thread");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Thread");

            migrationBuilder.DropColumn(
                name: "MatchId",
                table: "Thread");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "Thread");

            migrationBuilder.AlterColumn<string>(
                name: "Hand",
                table: "Thread",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
