using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSC_Backend.Migrations
{
    /// <inheritdoc />
    public partial class initialskrillex23 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImdbRating",
                table: "movies");

            migrationBuilder.AddColumn<string>(
                name: "ImdbId",
                table: "movies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RatingSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingSources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatingSources_movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "movies",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                column: "ImdbId",
                value: "");

            migrationBuilder.CreateIndex(
                name: "IX_RatingSources_MovieId_Source",
                table: "RatingSources",
                columns: new[] { "MovieId", "Source" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RatingSources");

            migrationBuilder.DropColumn(
                name: "ImdbId",
                table: "movies");

            migrationBuilder.AddColumn<float>(
                name: "ImdbRating",
                table: "movies",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "movies",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"),
                column: "ImdbRating",
                value: 0f);
        }
    }
}
