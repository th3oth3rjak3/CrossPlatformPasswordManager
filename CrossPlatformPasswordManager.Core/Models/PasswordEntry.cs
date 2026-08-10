using System.ComponentModel.DataAnnotations;

namespace CrossPlatformPasswordManager.Core.Models;

public class PasswordEntry
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(256)]
    public required string SiteName { get; set; }

    [Required]
    [MaxLength(256)]
    public required string Username { get; set; }

    [Required]
    [MaxLength(256)]
    public required string PasswordHash { get; set; }
}