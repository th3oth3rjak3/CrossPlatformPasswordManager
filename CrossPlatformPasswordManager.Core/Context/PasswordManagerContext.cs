using CrossPlatformPasswordManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CrossPlatformPasswordManager.Core.Context;

public partial class PasswordManagerContext(DbContextOptions<PasswordManagerContext> options) : DbContext(options)
{
    public DbSet<MasterPassword> MasterPasswords { get; set; } = null!;
    public DbSet<PasswordEntry> PasswordEntries { get; set; } = null!;
}