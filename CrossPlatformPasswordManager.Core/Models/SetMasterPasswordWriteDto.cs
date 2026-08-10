using System.ComponentModel.DataAnnotations;

namespace CrossPlatformPasswordManager.Core.Models;

public class SetMasterPasswordWriteDto
{
    [Required]
    [MinLength(5)]
    public string? MasterPassword { get; set; }

    [Required]
    [MinLength(5)]
    public string? ConfirmPassword { get; set; }
}