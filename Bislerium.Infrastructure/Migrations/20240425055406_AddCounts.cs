using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bislerium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DownVotes",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpVotes",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Comments",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DownVotes",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UpVotes",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DownVotes",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UpVotes",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "DownVotes",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "UpVotes",
                table: "Blogs");
        }
    }
}
