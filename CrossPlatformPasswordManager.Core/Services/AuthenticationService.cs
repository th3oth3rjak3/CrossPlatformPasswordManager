using CrossPlatformPasswordManager.Core.Models;

namespace CrossPlatformPasswordManager.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly VaultSession _session;

    public AuthenticationService(VaultSession session)
    {
        _session = session;
    }

    public Task<AuthenticationState> GetCurrentStateAsync()
    {
        if (!_session.IsMasterPasswordSet)
        {
            return Task.FromResult(AuthenticationState.SetMasterPasswordRequired);
        }

        if (!_session.IsLoggedIn)
        {
            return Task.FromResult(AuthenticationState.UnlockRequired);
        }

        return Task.FromResult(AuthenticationState.Authenticated);
    }

    public Task<bool> IsMasterPasswordSetAsync() => Task.FromResult(_session.IsMasterPasswordSet);

    public bool IsVaultUnlocked() => _session.IsLoggedIn;

    public Task SetMasterPasswordAsync(string password)
    {
        // TODO: Hash password using Argon2/PBKDF2 and save salt + hash to SQLite
        _session.MasterPasswordHash = "dummy_hash"; // Placeholder until SQLite is linked
        _session.IsLoggedIn = true;
        return Task.CompletedTask;
    }

    public Task<bool> UnlockVaultAsync(string password)
    {
        // TODO: Validate entered password against stored SQLite hash
        _session.IsLoggedIn = true;
        return Task.FromResult(true);
    }

    public void LockVault()
    {
        _session.ClearSensitiveData();
    }
}
