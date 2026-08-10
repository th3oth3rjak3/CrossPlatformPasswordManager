using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CrossPlatformPasswordManager.Core.Context;

public class PasswordManagerContextFactory : IDesignTimeDbContextFactory<PasswordManagerContext>
{
    public PasswordManagerContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PasswordManagerContext>();

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PasswordManager",
            "PasswordManager.sqlite");

        _ = optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new PasswordManagerContext(optionsBuilder.Options);
    }
}