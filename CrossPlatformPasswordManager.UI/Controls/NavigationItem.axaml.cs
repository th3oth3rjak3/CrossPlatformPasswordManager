using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CrossPlatformPasswordManager.UI.Models;

namespace CrossPlatformPasswordManager.UI.Controls;

public partial class NavigationItem : UserControl
{
    public static readonly StyledProperty<AppIcon> IconProperty =
        AvaloniaProperty.Register<NavigationItem, AppIcon>(
            nameof(Icon));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<NavigationItem, string>(
            nameof(Text),
            string.Empty);

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<NavigationItem, ICommand?>(
            nameof(Command));

    public static readonly StyledProperty<string> ButtonClassProperty =
        AvaloniaProperty.Register<NavigationItem, string>(
            nameof(ButtonClass),
            string.Empty);

    public static readonly DirectProperty<NavigationItem, string> IconSourceProperty =
        AvaloniaProperty.RegisterDirect<NavigationItem, string>(
            nameof(IconSource),
            o => o.IconSource);

    public AppIcon Icon
    {
        get => GetValue(IconProperty);
        set
        {
            SetValue(IconProperty, value);
            IconSource = value.GetSource();
        }
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public string ButtonClass
    {
        get => GetValue(ButtonClassProperty);
        set => SetValue(ButtonClassProperty, value);
    }

    public string IconSource
    {
        get;
        private set => SetAndRaise(IconSourceProperty, ref field, value);
    }

    public NavigationItem()
    {
        InitializeComponent();

        IconSource = Icon.GetSource();

        ButtonClassProperty.Changed.AddClassHandler<NavigationItem>(
            static (control, _) => control.UpdateButtonClasses());

        UpdateButtonClasses();
    }
    
    private void UpdateButtonClasses()
    {
        Button.Classes.Clear();

        foreach (var className in ButtonClass.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            Button.Classes.Add(className);
        }
    }
}
