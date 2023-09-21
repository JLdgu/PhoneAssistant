using Moq;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

using Xunit;

namespace PhoneAssistant.Tests.Features.Settings;

public sealed class SettingsMainViewModelTests
{
    [Fact]
    public void Constructor_SetsVMProperties()
    {
        const string VERSION = "0.0.0.1";
        Mock<IUserSettings> userSettings = new();
        userSettings.Setup(s => s.AssemblyVersion).Returns(new Version(VERSION));
        userSettings.Setup(s => s.PrintToFile).Returns(false);
        userSettings.Setup(s => s.Printer).Returns("Printer");
        userSettings.Setup(s => s.PrintFile).Returns("PrintFile");
        userSettings.Setup(s => s.DarkMode).Returns(true);
        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(userSettings.Object, themeWrapper.Object);

        Xunit.Assert.True(vm.PrintToPrinter);
        Xunit.Assert.False(vm.PrintToFile);
        Xunit.Assert.Equal("Printer", vm.Printer);
        Xunit.Assert.Equal("PrintFile", vm.PrintFile);

        Xunit.Assert.False(vm.ColourThemeLight);
        Xunit.Assert.True(vm.ColourThemeDark);

        Xunit.Assert.Equal(VERSION, vm.VersionDescription);

        userSettings.VerifyGet(s => s.AssemblyVersion);
    }

    [Fact]
    public void Constructor_SettingPrintToFileFalse_SetsPrintToPrinterTrue()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.PrintToFile).Returns(false);
        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(mock.Object, themeWrapper.Object);


        Xunit.Assert.True(vm.PrintToPrinter);
        Xunit.Assert.False(vm.PrintToFile);
    }

    [Fact]
    public void Constructor_SettingPrintToFileTrue_SetsPrintToFileTrue()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.PrintToFile).Returns(true);

        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(mock.Object, themeWrapper.Object);


        Xunit.Assert.True(vm.PrintToFile);
        Xunit.Assert.False(vm.PrintToPrinter);
    }

    [Fact]
    public void Database_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.Database).Returns("Database");
        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(mock.Object, themeWrapper.Object);

        const string NEW_VALUE = "Database Changed";

        vm.Database = NEW_VALUE;

        mock.VerifySet(s => s.Database = NEW_VALUE);
        mock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void Printer_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.Printer).Returns("Printer");
        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(mock.Object, themeWrapper.Object);

        const string NEW_VALUE = "Printer Changed";

        vm.Printer = NEW_VALUE;

        mock.VerifySet(s => s.Printer = NEW_VALUE);
        mock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void PrinterFile_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.PrintFile).Returns("PrintFile");
        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(mock.Object, themeWrapper.Object);

        const string NEW_VALUE = "PrintFile Changed";

        vm.PrintFile = NEW_VALUE;

        mock.VerifySet(s => s.PrintFile = NEW_VALUE);
        mock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void ColourThemeDark_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.DarkMode).Returns(false);
        Mock<IThemeWrapper> themeWrapper = new();
        var vm = new SettingsMainViewModel(mock.Object, themeWrapper.Object);

        vm.ColourThemeDark = true;

        mock.VerifySet(s => s.DarkMode = true);
        mock.Verify(s => s.Save(), Times.Once());

    }
}