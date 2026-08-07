namespace CrossPlatformPasswordManager.Core.Models;

/// <summary>
/// Represents the overall authentication state of the application for view routing.
/// </summary>
public enum AuthenticationState
{
    SetMasterPasswordRequired,
    UnlockRequired,
    Authenticated
}
