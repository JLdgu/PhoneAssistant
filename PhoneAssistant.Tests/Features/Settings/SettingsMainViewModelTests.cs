﻿using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

using Xunit;

namespace PhoneAssistant.Tests.Features.Settings;

public sealed class SettingsMainViewModelTests
{
    [Theory]
    [InlineData(true, false, true)]
    [InlineData(false, true ,false)]
    public void Constructor_SetsVMProperties(bool printToFile, bool dymoPrintToFile, bool darkMode)
    {
        const string VERSION = "0.0.0.1";
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userSettings = mocker.GetMock<IUserSettings>();
        userSettings.Setup(s => s.Database).Returns("Database");
        userSettings.Setup(s => s.PrintToFile).Returns(printToFile);
        userSettings.Setup(s => s.Printer).Returns("Printer");
        userSettings.Setup(s => s.PrintFile).Returns("PrintFile");
        userSettings.Setup(s => s.DymoPrintToFile).Returns(dymoPrintToFile);
        userSettings.Setup(s => s.DymoPrinter).Returns("DymoPrinter");
        userSettings.Setup(s => s.DymoPrintFile).Returns("DymoPrintFile");
        userSettings.Setup(s => s.DarkMode).Returns(darkMode);
        userSettings.Setup(s => s.AssemblyVersion).Returns(new Version(VERSION));
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        Assert.Equal(printToFile, vm.PrintToFile);
        Assert.Equal(!printToFile, vm.PrintToPrinter);
        Assert.Equal("Printer", vm.Printer);
        Assert.Equal("PrintFile", vm.PrintFile);

        Assert.Equal(dymoPrintToFile, vm.DymoPrintToFile);
        Assert.Equal(!dymoPrintToFile, vm.DymoPrintToPrinter);
        Assert.Equal("DymoPrinter", vm.DymoPrinter);
        Assert.Equal("DymoPrintFile", vm.DymoPrintFile);

        Assert.Equal(darkMode,vm.ColourThemeDark);
        Assert.Equal(!darkMode,vm.ColourThemeLight);

        Assert.Equal(VERSION, vm.CurrentVersion);
        userSettings.VerifyGet(s => s.AssemblyVersion);
    }

    [Fact]
    public void Database_PropertyChanged_SavesUpdatedSettings()
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userStettingsMock = mocker.GetMock<IUserSettings>();
        userStettingsMock.Setup(s => s.Database).Returns("Database");
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "Database Changed";
        vm.Database = NEW_VALUE;
        
        userStettingsMock.VerifySet(s => s.Database = NEW_VALUE);
        userStettingsMock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void Printer_PropertyChanged_SavesUpdatedSettings()
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userStettingsMock = mocker.GetMock<IUserSettings>();
        userStettingsMock.Setup(s => s.Printer).Returns("Printer");
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "Printer Changed";
        vm.Printer = NEW_VALUE;

        userStettingsMock.VerifySet(s => s.Printer = NEW_VALUE);
        userStettingsMock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void PrinterFile_PropertyChanged_SavesUpdatedSettings()
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userStettingsMock = mocker.GetMock<IUserSettings>();
        userStettingsMock.Setup(s => s.PrintFile).Returns("PrintFile");
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "PrintFile Changed";
        vm.PrintFile = NEW_VALUE;

        userStettingsMock.VerifySet(s => s.PrintFile = NEW_VALUE);
        userStettingsMock.Verify(s => s.Save(), Times.Once());
    }
    [Fact]
    public void DymoPrinter_PropertyChanged_SavesUpdatedSettings()
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userStettingsMock = mocker.GetMock<IUserSettings>();
        userStettingsMock.Setup(s => s.DymoPrinter).Returns("DymoPrinter");
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "Printer Changed";
        vm.DymoPrinter = NEW_VALUE;

        userStettingsMock.VerifySet(s => s.DymoPrinter = NEW_VALUE);
        userStettingsMock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void DymoPrinterFile_PropertyChanged_SavesUpdatedSettings()
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userStettingsMock = mocker.GetMock<IUserSettings>();
        userStettingsMock.Setup(s => s.DymoPrintFile).Returns("DymoPrintFile");
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "PrintFile Changed";
        vm.DymoPrintFile = NEW_VALUE;

        userStettingsMock.VerifySet(s => s.DymoPrintFile = NEW_VALUE);
        userStettingsMock.Verify(s => s.Save(), Times.Once());
    }

    [Fact]
    public void ColourThemeDark_PropertyChanged_SavesUpdatedSettings()
    {
        AutoMocker mocker = new AutoMocker();
        Mock<IUserSettings> userStettingsMock = mocker.GetMock<IUserSettings>();
        userStettingsMock.Setup(s => s.DarkMode).Returns(false);
        Mock<IThemeWrapper> themeWrapperMock = mocker.GetMock<IThemeWrapper>();
        SettingsMainViewModel vm = mocker.CreateInstance<SettingsMainViewModel>();

        vm.ColourThemeDark = true;

        userStettingsMock.VerifySet(s => s.DarkMode = true);
        userStettingsMock.Verify(s => s.Save(), Times.Once());
        themeWrapperMock.Verify(t => t.ModifyTheme(true),Times.Once());
    }
}