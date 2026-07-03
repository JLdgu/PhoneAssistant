using System.Windows.Media;

using MaterialDesignThemes.Wpf;

namespace PhoneAssistant.WPF.Features.Settings;

public interface IThemeWrapper
{
    /// <summary>
    /// Modifies the theme based on the dark theme setting.
    /// </summary>
    /// <param name="isDarkTheme">Indicates whether the dark theme is enabled.</param>
    void ModifyTheme(bool isDarkTheme);

    /// <summary>
    /// Modifies the theme with custom primary and secondary colors.
    /// </summary>
    /// <param name="isDarkTheme">Indicates whether the dark theme is enabled.</param>
    /// <param name="primaryColor">The primary color for the theme.</param>
    /// <param name="secondaryColor">The secondary color for the theme.</param>
    void ModifyTheme(bool isDarkTheme, Color? primaryColor, Color? secondaryColor);
}

public sealed class ThemeWrapper : IThemeWrapper
{
    public void ModifyTheme(bool isDarkTheme)
    { 
        ModifyTheme(isDarkTheme, Colors.Cyan, Colors.Teal);
    }

    public void ModifyTheme(bool isDarkTheme, Color? primaryColor, Color? secondaryColor)
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        theme.SetPrimaryColor(primaryColor ?? Colors.Cyan);
        theme.SetSecondaryColor(secondaryColor ?? Colors.Teal);
        theme.SetBaseTheme(isDarkTheme ? BaseTheme.Dark : BaseTheme.Light);
        paletteHelper.SetTheme(theme);
    }
}
