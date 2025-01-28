using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RockPaperScissors.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchBet = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.MatchId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "MatchHistory",
                columns: table => new
                {
                    MatchId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerOneId = table.Column<Guid>(type: "uuid", nullable: true),
                    PlayerTwoId = table.Column<Guid>(type: "uuid", nullable: true),
                    AmountBet = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    PlayerOneMove = table.Column<string>(type: "text", nullable: true),
                    PlayerTwoMove = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchHistory", x => x.MatchId);
                    table.ForeignKey(
                        name: "FK_MatchHistory_Users_PlayerOneId",
                        column: x => x.PlayerOneId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MatchHistory_Users_PlayerTwoId",
                        column: x => x.PlayerTwoId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameTransactions",
                columns: table => new
                {
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTransactions", x => x.TransactionId);
                    table.ForeignKey(
                        name: "FK_GameTransactions_MatchHistory_GameId",
                        column: x => x.GameId,
                        principalTable: "MatchHistory",
                        principalColumn: "MatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameTransactions_Users_FromUserId",
                        column: x => x.FromUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_GameTransactions_Users_ToUserId",
                        column: x => x.ToUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameTransactions_FromUserId_ToUserId",
                table: "GameTransactions",
                columns: new[] { "FromUserId", "ToUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_GameTransactions_GameId",
                table: "GameTransactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTransactions_ToUserId",
                table: "GameTransactions",
                column: "ToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_MatchId",
                table: "Matches",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchHistory_PlayerOneId",
                table: "MatchHistory",
                column: "PlayerOneId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchHistory_PlayerTwoId",
                table: "MatchHistory",
                column: "PlayerTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTransactions");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "MatchHistory");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
