using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bislerium.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModelsForBlogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBlog = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogHistories_Blogs_UpdatedBlog",
                        column: x => x.UpdatedBlog,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentBlog = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_CommentUser",
                        column: x => x.CommentUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Comments_Blogs_CommentBlog",
                        column: x => x.CommentBlog,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LikeBlogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reaction = table.Column<int>(type: "int", nullable: false),
                    LikedBlog = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LikedUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeBlogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeBlogs_AspNetUsers_LikedUser",
                        column: x => x.LikedUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_LikeBlogs_Blogs_LikedBlog",
                        column: x => x.LikedBlog,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedComment = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentHistories_Comments_UpdatedComment",
                        column: x => x.UpdatedComment,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UpvoteComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reaction = table.Column<int>(type: "int", nullable: false),
                    LikedComment = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LikedUser = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UpvoteComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UpvoteComments_AspNetUsers_LikedUser",
                        column: x => x.LikedUser,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_UpvoteComments_Comments_LikedComment",
                        column: x => x.LikedComment,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogHistories_UpdatedBlog",
                table: "BlogHistories",
                column: "UpdatedBlog");

            migrationBuilder.CreateIndex(
                name: "IX_CommentHistories_UpdatedComment",
                table: "CommentHistories",
                column: "UpdatedComment");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentBlog",
                table: "Comments",
                column: "CommentBlog");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentUser",
                table: "Comments",
                column: "CommentUser");

            migrationBuilder.CreateIndex(
                name: "IX_LikeBlogs_LikedBlog",
                table: "LikeBlogs",
                column: "LikedBlog");

            migrationBuilder.CreateIndex(
                name: "IX_LikeBlogs_LikedUser",
                table: "LikeBlogs",
                column: "LikedUser");

            migrationBuilder.CreateIndex(
                name: "IX_UpvoteComments_LikedComment",
                table: "UpvoteComments",
                column: "LikedComment");

            migrationBuilder.CreateIndex(
                name: "IX_UpvoteComments_LikedUser",
                table: "UpvoteComments",
                column: "LikedUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogHistories");

            migrationBuilder.DropTable(
                name: "CommentHistories");

            migrationBuilder.DropTable(
                name: "LikeBlogs");

            migrationBuilder.DropTable(
                name: "UpvoteComments");

            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
