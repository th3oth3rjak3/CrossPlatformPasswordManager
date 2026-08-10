using System.ComponentModel.DataAnnotations;

namespace CrossPlatformPasswordManager.Core.Models;

public class PasswordEntryWriteDto
{
    [Required]
    [StringLength(256)]
    public string? Site { get; set; }

    [Required]
    [StringLength(256)]
    public string? Username { get; set; }

    [Required]
    [StringLength(256)]
    public string? Password { get; set; }
}