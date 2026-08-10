using CrossPlatformPasswordManager.Core.Context;
using CrossPlatformPasswordManager.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace CrossPlatformPasswordManager.Core.Services;

public class MasterPasswordService(
    IDbContextFactory<PasswordManagerContext> contextFactory,
    DatabaseConnectionManager connectionManager,
    VaultSession vaultSession,
    IAuthenticationService authService
)
{
    public async Task<Result<Unit, Exception>> SetMasterPassword(string password) =>
        await TryAsync(async () =>
        {
            var backupDbPath = await connectionManager.CreateBackupAsync();
            await using var context = await contextFactory.CreateDbContextAsync();

            var hash = Crypto.HashMasterPassword(password);
            var newKeyDerivationSalt = Crypto.GenerateBase64Salt(16);
            var newAesEncryptionKey = Crypto.DeriveAesKey(password, newKeyDerivationSalt);
            var oldMasterPassword = await context.MasterPasswords.SingleAsync();

            var allPasswordEntries = await context.PasswordEntries.ToListAsync();

            await using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                foreach (var entry in allPasswordEntries)
                {
                    var plaintext = Crypto.DecryptEntry(vaultSession.AesEncryptionKey, entry.PasswordHash);
                    entry.PasswordHash = Crypto.EncryptEntry(newAesEncryptionKey, plaintext);
                }

                oldMasterPassword.PasswordHash = hash;
                oldMasterPassword.KeyDerivationSalt = newKeyDerivationSalt;

                _ = await context.SaveChangesAsync();
                await tx.CommitAsync();

                vaultSession.KeyDerivationSalt = newKeyDerivationSalt;
                vaultSession.MasterPasswordHash = hash;
            }
            catch (Exception)
            {
                await tx.RollbackAsync();
            }
        });
}