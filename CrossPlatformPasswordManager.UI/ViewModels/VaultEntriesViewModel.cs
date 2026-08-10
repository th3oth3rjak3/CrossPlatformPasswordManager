using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using CrossPlatformPasswordManager.Core.Models;
using CrossPlatformPasswordManager.Core.Services;

using Functional;

namespace CrossPlatformPasswordManager.UI.ViewModels;

public partial class VaultEntriesViewModel : ViewModelBase
{
    private readonly PasswordEntryService _passwordEntryService;
    private readonly ObservableCollection<PasswordEntryDisplayItem> _allEntries = [];

    [ObservableProperty]
    public partial ObservableCollection<PasswordEntryDisplayItem> FilteredEntries { get; set; } = [];

    [ObservableProperty]
    public partial string SearchQuery { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ViewModelBase? ActiveEditor { get; set; }

    public IAsyncRelayCommand LoadEntriesCommand { get; }
    public IRelayCommand AddPasswordCommand { get; }

    public VaultEntriesViewModel(PasswordEntryService passwordEntryService)
    {
        _passwordEntryService = passwordEntryService;

        LoadEntriesCommand = new AsyncRelayCommand(LoadEntriesAsync);
        AddPasswordCommand = new RelayCommand(AddPassword);

        _ = LoadEntriesAsync();
    }

    public async Task LoadEntriesAsync()
    {
        await _passwordEntryService
            .GetAllPasswordEntriesAsync()
            .EffectOkAsync(entries =>
            {
                _allEntries.Clear();
                foreach (var dto in entries)
                {
                    _allEntries.Add(new PasswordEntryDisplayItem(
                        dto.Id,
                        dto.Site,
                        dto.Username,
                        dto.Password,
                        onEdit: EditPassword,
                        onDelete: (item) => _ = DeletePasswordAsync(item)
                    ));
                }
                FilterEntries();
            });
    }

    private void AddPassword()
    {
        var editor = new PasswordEditorViewModel();

        editor.SaveRequested += async (writeDto) =>
        {
            if (writeDto != null)
            {
                await _passwordEntryService
                    .CreatePasswordEntryAsync(writeDto)
                    .EffectAsync(async _ => await LoadEntriesAsync());
            }

            ActiveEditor = null;
        };

        ActiveEditor = editor;
    }

    private void EditPassword(PasswordEntryDisplayItem? item)
    {
        if (item == null)
            return;

        var readDto = new PasswordEntryReadDto
        {
            Id = item.Id,
            Site = item.SiteName,
            Username = item.Username,
            Password = item.PlaintextPassword
        };

        var editor = new PasswordEditorViewModel(readDto);

        editor.SaveRequested += async (writeDto) =>
        {
            if (writeDto != null)
            {
                await _passwordEntryService
                    .UpdatePasswordEntryAsync(item.Id, writeDto)
                    .EffectAsync(async _ => await LoadEntriesAsync());
            }

            ActiveEditor = null;
        };

        ActiveEditor = editor;
    }

    private async Task DeletePasswordAsync(PasswordEntryDisplayItem? item)
    {
        if (item == null)
            return;

        // Create delete confirmation dialog
        var dialog = new ConfirmDialogViewModel(
            title: "Delete Password Entry?",
            message: $"Are you sure you want to delete '{item.SiteName}'? This action cannot be undone.",
            confirmButtonText: "Delete",
            severity: ConfirmDialogSeverity.Danger
        );

        dialog.CloseRequested += async (confirmed) =>
        {
            if (confirmed)
            {
                await _passwordEntryService
                    .DeletePasswordEntryAsync(item.Id)
                    .EffectAsync(async _ =>
                    {
                        _allEntries.Remove(item);
                        FilterEntries();
                        await Task.CompletedTask;
                    });
            }

            ActiveEditor = null;
        };

        // Open confirmation overlay
        ActiveEditor = dialog;
    }

    partial void OnSearchQueryChanged(string value) => FilterEntries();

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
}
