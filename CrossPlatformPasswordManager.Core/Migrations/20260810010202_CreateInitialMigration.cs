using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossPlatformPasswordManager.Core.Migrations
{
    /// <inheritdoc />
    public partial class CreateInitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MasterPasswords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    KeyDerivationSalt = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasterPasswords", x => x.Id);
                });

            _ = migrationBuilder.Sql(@"
insert into MasterPasswords (Id, PasswordHash, KeyDerivationSalt) values
(1, '', '');
");
            
            migrationBuilder.CreateTable(
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
                    table.PrimaryKey("PK_PasswordEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MasterPasswords");

            migrationBuilder.DropTable(
                name: "PasswordEntries");
        }
    }
}
