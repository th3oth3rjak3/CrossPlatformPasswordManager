using System.Security.Cryptography;

namespace CrossPlatformPasswordManager.Core.Models;

/// <summary>
/// Holds the active in-memory cryptographic state and encryption keys for an unlocked session.
/// </summary>
public class VaultSession
{
    private byte[] _aesEncryptionKey = new byte[16];

    public bool IsLoggedIn { get; set; }
    public string MasterPasswordHash { get; set; } = string.Empty;
    public string KeyDerivationSalt { get; set; } = string.Empty;

    public byte[] AesEncryptionKey
    {
        get => _aesEncryptionKey;
        set => _aesEncryptionKey = value ?? new byte[16];
    }

    public bool IsMasterPasswordSet => !string.IsNullOrEmpty(MasterPasswordHash);

    /// <summary>
    /// Securely clears sensitive key material from memory.
    /// </summary>
    public void ClearSensitiveData()
    {
        IsLoggedIn = false;

        if (_aesEncryptionKey.Length > 0)
        {
            CryptographicOperations.ZeroMemory(_aesEncryptionKey);
        }

        MasterPasswordHash = string.Empty;
        KeyDerivationSalt = string.Empty;
    }
}
