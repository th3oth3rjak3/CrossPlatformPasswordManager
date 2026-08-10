using Microsoft.Data.Sqlite;

namespace CrossPlatformPasswordManager.Core.Services;

public class DatabaseConnectionManager
{
    private readonly string _appDataFolder;
    private readonly string _dbPath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public DatabaseConnectionManager()
    {
        _appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PasswordManager"
        );

        _ = Directory.CreateDirectory(_appDataFolder);

        _dbPath = Path.Combine(_appDataFolder, "PasswordManager.sqlite");
    }

    public string DatabasePath => _dbPath;

    public async Task CloseAllConnectionsAsync()
    {
        await _lock.WaitAsync();
        try
        {
            SqliteConnection.ClearAllPools();
            await Task.Delay(100);
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    public async Task<FileInfo> CreateBackupAsync()
    {
        await _lock.WaitAsync();
        try
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var filename = $"PasswordManager_backup_{timestamp}.sqlite";
            var backupPath = Path.Combine(_appDataFolder, filename);

            // Use SQLite's built-in backup API to ensure consistency
            await using var sourceConnection = new SqliteConnection($"Data Source={_dbPath}");
            await using var backupConnection = new SqliteConnection($"Data Source={backupPath}");

            await sourceConnection.OpenAsync();
            await backupConnection.OpenAsync();

            sourceConnection.BackupDatabase(backupConnection);

            return new FileInfo(backupPath);
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    public async Task RestoreFromBackupAsync(string backupPath)
    {
        await _lock.WaitAsync();
        try
        {
            // Use SQLite's backup API to restore (reverse of backup)
            await using var backupConnection = new SqliteConnection($"Data Source={backupPath}");
            await using var targetConnection = new SqliteConnection($"Data Source={_dbPath}");

            await backupConnection.OpenAsync();
            await targetConnection.OpenAsync();

            // Copy FROM backup TO current database
            backupConnection.BackupDatabase(targetConnection);

            // Clear pool to ensure fresh connections after restore
            SqliteConnection.ClearAllPools();
        }
        finally
        {
            _ = _lock.Release();
        }
    }

    public async Task<Result<List<FileInfo>, Exception>> GetAllBackupsAsync() =>
        await TryAsync(() =>
            new DirectoryInfo(_appDataFolder)
                .EnumerateFiles()
                .Where(file => file.Extension == ".sqlite")
                .Where(file => file.FullName.Contains("backup", StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(file => file.Name)
                .ToList()
                .Async());
}