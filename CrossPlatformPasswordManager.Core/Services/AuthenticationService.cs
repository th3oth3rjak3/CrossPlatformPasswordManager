using CrossPlatformPasswordManager.Core.Models;

namespace CrossPlatformPasswordManager.Core.Services;

public class AuthenticationService(VaultSession session) : IAuthenticationService
{
    public Task<AuthenticationState> GetCurrentStateAsync()
    {
        if (!session.IsMasterPasswordSet)
        {
            return Task.FromResult(AuthenticationState.SetMasterPasswordRequired);
        }

        if (!session.IsLoggedIn)
        {
            return Task.FromResult(AuthenticationState.UnlockRequired);
        }

        return Task.FromResult(AuthenticationState.Authenticated);
    }

    public Task<bool> IsMasterPasswordSetAsync() => Task.FromResult(session.IsMasterPasswordSet);

    public bool IsVaultUnlocked() => session.IsLoggedIn;

    public Task SetMasterPasswordAsync(string password)
    {
        // TODO: Hash password using Argon2/PBKDF2 and save salt + hash to SQLite
        session.MasterPasswordHash = "dummy_hash"; // Placeholder until SQLite is linked
        session.IsLoggedIn = true;
        return Task.CompletedTask;
    }

    public Task<bool> UnlockVaultAsync(string password)
    {
        // TODO: Validate entered password against stored SQLite hash
        session.IsLoggedIn = true;
        return Task.FromResult(true);
    }

    public void LockVault()
    {
        session.ClearSensitiveData();
    }
}
