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
    public AuthenticationState GetCurrentState();

    /// <summary>
    /// Reload the authentication state from the database.
    /// </summary>
    public Task<Result<Unit, Exception>> ReloadAllAuthState();
    
    /// <summary>
    /// Hashes and stores a new master password, initializing the vault.
    /// </summary>
    /// <param name="masterPassword">The master password to configure.</param>
    public string? Login(string masterPassword);

    /// <summary>
    /// Immediately locks the vault and purges sensitive key material from memory.
    /// </summary>
    public void Logout();
}
