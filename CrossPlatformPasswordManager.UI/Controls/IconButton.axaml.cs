using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using CrossPlatformPasswordManager.UI.Models;

namespace CrossPlatformPasswordManager.UI.Controls;

public partial class IconButton : UserControl
{
    public static readonly StyledProperty<AppIcon> IconProperty =
        AvaloniaProperty.Register<IconButton, AppIcon>(nameof(Icon));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<IconButton, ICommand?>(nameof(Command));

    public static readonly StyledProperty<string> ButtonClassProperty =
        AvaloniaProperty.Register<IconButton, string>(
            nameof(ButtonClass),
            string.Empty);

    public static readonly StyledProperty<double> IconSizeProperty =
        AvaloniaProperty.Register<IconButton, double>(
            nameof(IconSize),
            16);

    public static readonly StyledProperty<string?> ToolTipProperty =
        AvaloniaProperty.Register<IconButton, string?>(
            nameof(ToolTip));

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<IconButton, string>(
            nameof(Text),
            string.Empty);
    
    public static readonly DirectProperty<IconButton, string> IconSourceProperty =
        AvaloniaProperty.RegisterDirect<IconButton, string>(
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

    public double IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? ToolTip
    {
        get => GetValue(ToolTipProperty);
        set => SetValue(ToolTipProperty, value);
    }

    public string IconSource
    {
        get;
        private set => SetAndRaise(IconSourceProperty, ref field, value);
    }

    public IconButton()
    {
        InitializeComponent();

        IconSource = Icon.GetSource();
        
        ButtonClassProperty.Changed.AddClassHandler<IconButton>(
            static (control, _) => control.UpdateButtonClasses());

        UpdateButtonClasses();
    }

    private void UpdateButtonClasses()
    {
        Button.Classes.Clear();

        foreach (var cl in ButtonClass.Split(
                     ' ',
                     StringSplitOptions.RemoveEmptyEntries))
        {
            Button.Classes.Add(cl);
        }
    }
}