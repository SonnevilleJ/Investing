using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Sonneville.Investing.Persistence.EFCore.Migrations
{
    public partial class Persist_salted_password_hashes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HashedDigest",
                columns: table => new
                {
                    DatabaseId = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Cryptor = table.Column<string>(nullable: true),
                    Digest = table.Column<byte[]>(nullable: true),
                    HashingAlgorithm = table.Column<string>(nullable: true),
                    Iterations = table.Column<int>(nullable: false),
                    SaltUsed = table.Column<byte[]>(nullable: true),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HashedDigest", x => x.DatabaseId);
                    table.ForeignKey(
                        name: "FK_HashedDigest_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUser",
                        principalColumn: "DatabaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HashedDigest_UserId",
                table: "HashedDigest",
                column: "UserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HashedDigest");
        }
    }
}
