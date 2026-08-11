namespace CrossPlatformPasswordManager.UI.Models;

public enum AppIcon
{
    Key,
    Lock,
    Save,
    LockWithKey,
    Search,
    Add,
    User,
    Copy,
    Eye,
    Edit,
    Delete,
    Restore,
}

public static class AppIconExtensions
{
    public static string GetSource(this AppIcon icon) => icon switch
    {
        AppIcon.Key => AppIcons.Key,
        AppIcon.Lock => AppIcons.Lock,
        AppIcon.Save => AppIcons.Save,
        AppIcon.LockWithKey => AppIcons.LockWithKey,
        AppIcon.Search => AppIcons.Search,
        AppIcon.Add => AppIcons.Add,
        AppIcon.User => AppIcons.User,
        AppIcon.Copy => AppIcons.Copy,
        AppIcon.Eye => AppIcons.Eye,
        AppIcon.Edit => AppIcons.Edit,
        AppIcon.Delete => AppIcons.Delete,
        AppIcon.Restore => AppIcons.Restore,
        _ => throw new ArgumentOutOfRangeException(nameof(icon))
    };
}

public static class AppIcons
{
    public static string Key => "\U0001F511";
    public static string Lock => "\U0001F512";
    public static string Save => "\U0001F4BE";
    public static string LockWithKey => "\U0001F510";
    public static string Search => "\U0001F50D";
    public static string Add => "\u2795";
    public static string User => "\U0001F464";
    public static string Copy => "\U0001F4CB";
    public static string Eye => "\U0001F441";
    public static string Edit => "\U0001F4DD";
    public static string Delete => "\U0001F5D1";
    public static string Restore => "\U0001F504";
}