namespace CrossPlatformPasswordManager.Core.Models;

public class PasswordEntryReadDto
{
    public required int Id { get; set; }
    public required string Site { get; set; }

    public required string Username { get; set; }

    public required string Password { get; set; }
}