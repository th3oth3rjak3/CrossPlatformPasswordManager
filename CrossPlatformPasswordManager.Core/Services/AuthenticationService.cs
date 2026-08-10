using System.Security.Cryptography;
using CrossPlatformPasswordManager.Core.Context;
using CrossPlatformPasswordManager.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace CrossPlatformPasswordManager.Core.Services;

public class AuthenticationService(
    VaultSession vaultSession,
    IdleTimerService idleTimer,
    IDbContextFactory<PasswordManagerContext> contextFactory
) : IAuthenticationService
{
    public AuthenticationState GetCurrentState()
    {
        if (!vaultSession.IsMasterPasswordSet) return AuthenticationState.SetMasterPasswordRequired;
        return vaultSession.IsLoggedIn ? AuthenticationState.Authenticated : AuthenticationState.UnlockRequired;
    }
    
    public async Task<Result<Unit, Exception>> ReloadAllAuthState() =>
        await TryAsync(() =>
        {
            using var context = contextFactory.CreateDbContext();
            var masterPw = context.MasterPasswords.First();
            vaultSession.MasterPasswordHash = masterPw.PasswordHash;
            vaultSession.AesEncryptionKey = new byte[16];
            vaultSession.KeyDerivationSalt = masterPw.KeyDerivationSalt;
            vaultSession.IsLoggedIn = false;
        });

    public string? Login(string masterPassword)
    {
        string? errorMessage = null;
        if (Crypto.VerifyMasterPassword(masterPassword, vaultSession.MasterPasswordHash))
        {
            vaultSession.AesEncryptionKey = Crypto.DeriveAesKey(masterPassword, vaultSession.KeyDerivationSalt);
            vaultSession.IsLoggedIn = true;
            idleTimer.StartTimer();
        }
        else
        {
            errorMessage = "Invalid password, please try again.";
        }

        return errorMessage;
    }

    public void Logout()
    {
        CryptographicOperations.ZeroMemory(vaultSession.AesEncryptionKey);
        vaultSession.AesEncryptionKey = new byte[16];
        vaultSession.IsLoggedIn = false;
        idleTimer.StopTimer();
    }
}