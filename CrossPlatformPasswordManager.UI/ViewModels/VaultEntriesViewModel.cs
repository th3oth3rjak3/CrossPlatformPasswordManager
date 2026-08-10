using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class VaultEntriesViewModel : ViewModelBase
{
    private readonly ObservableCollection<PasswordEntryDisplayItem> _allEntries = [];

    [ObservableProperty]
    public partial ObservableCollection<PasswordEntryDisplayItem> FilteredEntries { get; set; } = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    public IRelayCommand AddPasswordCommand { get; }
    public IRelayCommand<PasswordEntryDisplayItem> EditPasswordCommand { get; }
    public IRelayCommand<PasswordEntryDisplayItem> DeletePasswordCommand { get; }

    public VaultEntriesViewModel()
    {
        AddPasswordCommand = new RelayCommand(AddPassword);
        EditPasswordCommand = new RelayCommand<PasswordEntryDisplayItem>(EditPassword);
        DeletePasswordCommand = new RelayCommand<PasswordEntryDisplayItem>(DeletePassword);

        FilterEntries();
    }

    /// <summary>
    /// Used by Avalonia when setting SearchQuery. This allows us to hook into the setter and filter our entries list.
    /// </summary>
    /// <param name="value">The value of the search query.</param>
    partial void OnSearchQueryChanged(string value)
    {
        FilterEntries();
    }

    private void FilterEntries()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            FilteredEntries = new ObservableCollection<PasswordEntryDisplayItem>(_allEntries);
        }
        else
        {
            var filtered = _allEntries.Where(e =>
                e.SiteName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                e.Username.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

            FilteredEntries = new ObservableCollection<PasswordEntryDisplayItem>(filtered);
        }
    }

    private void AddPassword()
    {
        // Open modal/dialog to create a new password entry
    }

    private void EditPassword(PasswordEntryDisplayItem? item)
    {
        if (item == null)
            return;
        // Open modal/dialog to edit the selected password entry
    }

    private void DeletePassword(PasswordEntryDisplayItem? item)
    {
        if (item == null)
            return;
        _allEntries.Remove(item);
        FilterEntries();
    }
}
