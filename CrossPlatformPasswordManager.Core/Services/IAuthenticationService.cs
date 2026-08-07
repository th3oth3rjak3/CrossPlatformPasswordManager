using CrossPlatformPasswordManager.Core.Models;

namespace CrossPlatformPasswordManager.Core.Services;

/// <summary>
/// Manages master password authentication, vault lock/unlock states, and session persistence.
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Evaluates the current authentication state of the application.
    /// </summary>
    public Task<AuthenticationState> GetCurrentStateAsync();

    /// <summary>
    /// Checks whether a master password has been configured in persistent storage.
    /// </summary>
    public Task<bool> IsMasterPasswordSetAsync();

    /// <summary>
    /// Determines whether the vault is currently unlocked in memory.
    /// </summary>
    public bool IsVaultUnlocked();

    /// <summary>
    /// Hashes and stores a new master password, initializing the vault.
    /// </summary>
    /// <param name="password">The master password to configure.</param>
    public Task SetMasterPasswordAsync(string password);

    /// <summary>
    /// Verifies the provided password against the stored master password hash and unlocks the vault if valid.
    /// </summary>
    /// <param name="password">The password attempt entered by the user.</param>
    public Task<bool> UnlockVaultAsync(string password);

    /// <summary>
    /// Immediately locks the vault and purges sensitive key material from memory.
    /// </summary>
    public void LockVault();
}
