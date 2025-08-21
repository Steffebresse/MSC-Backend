using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSC_Backend.Migrations
{
    /// <inheritdoc />
    public partial class serbia13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscussionContent",
                table: "Discussions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscussionContent",
                table: "Discussions");
        }
    }
}
