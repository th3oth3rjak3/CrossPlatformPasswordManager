using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrossPlatformPasswordManager.Core.Migrations;

/// <inheritdoc />
public partial class CreateMasterPasswordTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        _ = migrationBuilder.CreateTable(
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
                _ = table.PrimaryKey("PK_MasterPasswords", x => x.Id);
            });

        _ = migrationBuilder.Sql(@"
insert into MasterPasswords (Id, PasswordHash, KeyDerivationSalt) values
(1, '', '');
");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(
            name: "MasterPasswords");
}
