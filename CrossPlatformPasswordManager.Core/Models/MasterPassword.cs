using System.ComponentModel.DataAnnotations;

namespace CrossPlatformPasswordManager.Core.Models;

public class MasterPassword
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string PasswordHash { get; set; }

    [Required]
    [MaxLength(256)]
    public required string KeyDerivationSalt { get; set; }
}