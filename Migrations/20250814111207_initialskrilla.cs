using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSC_Backend.Migrations
{
    /// <inheritdoc />
    public partial class initialskrilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MSCUserMovie");

            migrationBuilder.CreateTable(
                name: "ApplicationUserMovie",
                columns: table => new
                {
                    mSCUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    moviesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserMovie", x => new { x.mSCUsersId, x.moviesId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserMovie_AspNetUsers_mSCUsersId",
                        column: x => x.mSCUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserMovie_movies_moviesId",
                        column: x => x.moviesId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserMovie_moviesId",
                table: "ApplicationUserMovie",
                column: "moviesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserMovie");

            migrationBuilder.CreateTable(
                name: "MSCUserMovie",
                columns: table => new
                {
                    mSCUsersId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    moviesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MSCUserMovie", x => new { x.mSCUsersId, x.moviesId });
                    table.ForeignKey(
                        name: "FK_MSCUserMovie_AspNetUsers_mSCUsersId",
                        column: x => x.mSCUsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MSCUserMovie_movies_moviesId",
                        column: x => x.moviesId,
                        principalTable: "movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MSCUserMovie_moviesId",
                table: "MSCUserMovie",
                column: "moviesId");
        }
    }
}
