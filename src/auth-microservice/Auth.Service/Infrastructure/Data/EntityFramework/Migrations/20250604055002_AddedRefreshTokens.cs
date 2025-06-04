using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Auth.Service.Infrastructure.Data.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddedRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    ApplicationUserId = table.Column<string>(type: "text", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    Expires = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revoked = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => new { x.ApplicationUserId, x.Id });
                    table.ForeignKey(
                        name: "FK_RefreshToken_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2d6ebf06-bd20-4def-a812-4ac21c85cf79", "AQAAAAIAAYagAAAAENHdTrEGp9HBUdfoYleVuVwpGxkJh1Z6EMW1FYNdkV5xmrUXJjWMBqyrZ8P3RocyZA==", "3195ec3b-fbc1-42f9-bd2b-b09f84f3e113" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "64f73619-3b5d-411b-8213-1a0dcdfe5f00", "AQAAAAIAAYagAAAAEKdCyy/eYVE1dcAMxqoY4bFfDxIHLysjOzajvuRlnCyuqgRuwQ1PqJRcoG1RZHi2WQ==", "5b046b80-c8cd-4c26-99ea-cf28eba974fb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "8e445865-a24d-4543-a6c6-9443d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e8f27920-96b9-4f21-8292-85eedb460d0d", "AQAAAAIAAYagAAAAEL3Od2JQgYNq7oVjWzzBSsZW631DkNNv6/51fQCtkszA6hQ/mn5LwUotzVf/qm2USg==", "cdb6e2f6-3baf-4d78-8dc3-8b4575875dab" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "9e224968-33e4-4652-b7b7-8574d048cdb9",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "287eda56-dd8f-4a72-8c96-37e4a224e1c4", "AQAAAAIAAYagAAAAECerx2f8cQg/hfhxDfMJJBYGSSjBH5iJ0cucl0U3ud6aBnMJzWfkk0CaDEFRQTRXqA==", "eb8b774a-ea97-4fb1-8301-65c64ab90292" });
        }
    }
}
