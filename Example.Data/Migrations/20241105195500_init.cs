using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Example.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "example");

            migrationBuilder.CreateTable(
                name: "activity_type",
                schema: "example",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity_type", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "example",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    EmailVerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailVerificationToken = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EmailVerificationTokenValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "activity",
                schema: "example",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityTypeId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_activity_activity_type_ActivityTypeId",
                        column: x => x.ActivityTypeId,
                        principalSchema: "example",
                        principalTable: "activity_type",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_activity_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "example",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                schema: "example",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_UserId",
                        column: x => x.UserId,
                        principalSchema: "example",
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "example",
                table: "activity_type",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Login using name and password", "Login" },
                    { 2, "Register a new user", "Register" },
                    { 3, "Refresh token", "RefreshToken" },
                    { 4, "Logout", "Logout" },
                    { 5, "Email verification", "EmailVerification" },
                    { 6, "Password reset", "PasswordReset" },
                    { 7, "Password change", "PasswordChange" },
                    { 8, "User update", "UserUpdate" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_activity_ActivityTypeId",
                schema: "example",
                table: "activity",
                column: "ActivityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_activity_UserId",
                schema: "example",
                table: "activity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_UserId",
                schema: "example",
                table: "refresh_token",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_user_Email",
                schema: "example",
                table: "user",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "activity",
                schema: "example");

            migrationBuilder.DropTable(
                name: "refresh_token",
                schema: "example");

            migrationBuilder.DropTable(
                name: "activity_type",
                schema: "example");

            migrationBuilder.DropTable(
                name: "user",
                schema: "example");
        }
    }
}
