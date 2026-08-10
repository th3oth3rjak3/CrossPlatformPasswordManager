using CrossPlatformPasswordManager.Core.Context;
using CrossPlatformPasswordManager.Core.Models;

using Microsoft.EntityFrameworkCore;

namespace CrossPlatformPasswordManager.Core.Services;

public class PasswordEntryService(IDbContextFactory<PasswordManagerContext> contextFactory, VaultSession authState)
{
    public async Task<Result<Unit, Exception>> CreatePasswordEntryAsync(PasswordEntryWriteDto request) =>
        await TryAsync(async () =>
        {
            ArgumentNullException.ThrowIfNull(request.Username);
            ArgumentNullException.ThrowIfNull(request.Site);
            ArgumentNullException.ThrowIfNull(request.Password);

            var newEntry = new PasswordEntry()
            {
                Username = request.Username,
                SiteName = request.Site,
                PasswordHash = Crypto.EncryptEntry(authState.AesEncryptionKey, request.Password),
            };

            await using var context = await contextFactory.CreateDbContextAsync();

            _ = context.PasswordEntries.Add(newEntry);
            _ = await context.SaveChangesAsync();
        });

    public async Task<Result<Unit, Exception>> UpdatePasswordEntryAsync(int id, PasswordEntryWriteDto request) =>
        await TryAsync(async () =>
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var foundEntry = await context.PasswordEntries.SingleOrDefaultAsync(entry => entry.Id == id);

            ArgumentNullException.ThrowIfNull(request.Username);
            ArgumentNullException.ThrowIfNull(request.Site);
            ArgumentNullException.ThrowIfNull(request.Password);

            if (foundEntry is null)
            {
                throw new Exception($"Password entry with id '{id}' not found");
            }

            foundEntry.Username = request.Username;
            foundEntry.SiteName = request.Site;
            foundEntry.PasswordHash = Crypto.EncryptEntry(authState.AesEncryptionKey, request.Password);
            _ = await context.SaveChangesAsync();
        });

    public async Task<Result<Unit, Exception>> DeletePasswordEntryAsync(int id) =>
        await TryAsync(async () =>
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            _ = await context
                .PasswordEntries
                .Where(entry => entry.Id == id)
                .ExecuteDeleteAsync();
        });

    public async Task<Result<PasswordEntryReadDto, Exception>> GetPasswordEntryByIdAsync(int id) =>
        await TryAsync(async () =>
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var found = await context.PasswordEntries.FindAsync(id);
            if (found is null)
            {
                throw new Exception($"Password Entry with id '{id}' was not found.");
            }

            return new PasswordEntryReadDto
            {
                Id = found.Id,
                Site = found.SiteName,
                Username = found.Username,
                Password = Crypto.DecryptEntry(authState.AesEncryptionKey, found.PasswordHash),
            };
        });

    public async Task<Result<List<PasswordEntryReadDto>, Exception>> GetAllPasswordEntriesAsync() =>
        await TryAsync(async () =>
        {
            await using var context = await contextFactory.CreateDbContextAsync();
            var allEntries = await context.PasswordEntries.ToListAsync();
            return allEntries
                .Select(entry =>
                    new PasswordEntryReadDto
                    {
                        Id = entry.Id,
                        Site = entry.SiteName,
                        Username = entry.Username,
                        Password = Crypto.DecryptEntry(authState.AesEncryptionKey, entry.PasswordHash),
                    })
                .ToList();
        });
}