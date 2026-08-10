using CommunityToolkit.Mvvm.Input;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class BackupDisplayItem : ViewModelBase
{
    public FileInfo File { get; }

    public string FileName => File.Name;
    public string FullPath => File.FullName;
    public DateTime CreatedAt => File.CreationTime;
    public string FormattedSize => $"{File.Length / 1024.0:F1} KB";

    public IRelayCommand RestoreCommand { get; }
    public IRelayCommand DeleteCommand { get; }

    public BackupDisplayItem(FileInfo file, Action<BackupDisplayItem> onRestore, Action<BackupDisplayItem> onDelete)
    {
        File = file;
        RestoreCommand = new RelayCommand(() => onRestore(this));
        DeleteCommand = new RelayCommand(() => onDelete(this));
    }
}
