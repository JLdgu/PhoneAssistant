using MaterialDesignThemes.Wpf;

namespace PhoneAssistant.WPF.Features.Settings;
public sealed class ThemeWrapper : IThemeWrapper
{
    public void ModifyTheme(bool isDarkTheme)
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();

        theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
        paletteHelper.SetTheme(theme);
    }

}
