using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossPlatformPasswordManager.Core.Migrations;

/// <inheritdoc />
public partial class CreatePasswordEntriesTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) => migrationBuilder.CreateTable(
            name: "PasswordEntries",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                SiteName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                Username = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                _ = table.PrimaryKey("PK_PasswordEntries", x => x.Id);
            });

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(
            name: "PasswordEntries");
}
