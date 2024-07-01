using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Back.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usuario = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Numero = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Clave = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__2B3DE7B839539EFE", x => x.UsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Follows",
                columns: table => new
                {
                    FollowerId = table.Column<int>(type: "int", nullable: false),
                    FolloweeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Follows__67A1FC7C82A23FCE", x => new { x.FollowerId, x.FolloweeId });
                    table.ForeignKey(
                        name: "FK_Follows_FolloweeId_Users_UsuarioId",
                        column: x => x.FolloweeId,
                        principalTable: "Users",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_Follows_FollowerId_Users_UsuarioId",
                        column: x => x.FollowerId,
                        principalTable: "Users",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(280)", maxLength: 280, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", unicode: false, nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    UserUsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Posts__AA126018436802A7", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Post_Usuario",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "UsuarioId");
                    table.ForeignKey(
                        name: "FK_Posts_Users_UserUsuarioId",
                        column: x => x.UserUsuarioId,
                        principalTable: "Users",
                        principalColumn: "UsuarioId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FolloweeId",
                table: "Follows",
                column: "FolloweeId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserUsuarioId",
                table: "Posts",
                column: "UserUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UsuarioId",
                table: "Posts",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Numero",
                table: "Users",
                column: "Numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Usuario",
                table: "Users",
                column: "Usuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
