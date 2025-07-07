using Moq;
using Moq.AutoMock;
using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.Settings;
using Velopack;
using Velopack.Locators;

namespace PhoneAssistant.Tests.Features.Settings;

public sealed class SettingsMainViewModelTests
{
    private readonly AutoMocker _mocker = new();
    private readonly ApplicationSettings _appSettings = new();
    private Mock<IApplicationSettingsRepository> _applicationSettingsRepository;

    public SettingsMainViewModelTests()
    {
        _applicationSettingsRepository = _mocker.GetMock<IApplicationSettingsRepository>();
        _applicationSettingsRepository.Setup(s => s.ApplicationSettings).Returns(_appSettings);
    }

    [Test]
    public async Task ColourThemeDark_PropertyChanged_SavesUpdatedSettings()
    {
        _appSettings.DarkMode = false;
        Mock<IThemeWrapper> themeWrapperMock = _mocker.GetMock<IThemeWrapper>();
        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        vm.ColourThemeDark = true;

        await Assert.That(_appSettings.DarkMode).IsTrue();
        _applicationSettingsRepository.Verify(s => s.Save(), Times.Once());
        themeWrapperMock.Verify(t => t.ModifyTheme(true), Times.Once());
    }

    [Test]
    [Arguments(true, false, true)]
    [Arguments(false, true ,false)]
    public async Task Constructor_SetsVMPropertiesAsync(bool printToFile, bool dymoPrintToFile, bool darkMode)
    {
        _appSettings.DarkMode = darkMode;
        _appSettings.Database = "Database";
        _appSettings.DymoPrintToFile = dymoPrintToFile;
        _appSettings.DymoPrinter = "DymoPrinter";
        _appSettings.DymoPrintFile = "DymoPrintFile";
        _appSettings.PrintToFile = printToFile;
        _appSettings.Printer = "Printer";
        _appSettings.PrintFile = "PrintFile";
        _appSettings.ReleaseUrl = "url";

        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        await Assert.That(vm.ColourThemeDark).IsEqualTo(darkMode);
        await Assert.That(vm.ColourThemeLight).IsNotEqualTo(darkMode);

        await Assert.That(vm.DymoPrintToFile).IsEqualTo(dymoPrintToFile);
        await Assert.That(vm.DymoPrintToPrinter).IsEqualTo(!dymoPrintToFile);
        await Assert.That(vm.DymoPrinter).IsEqualTo("DymoPrinter");
        await Assert.That(vm.DymoPrintFile).IsEqualTo("DymoPrintFile");

        await Assert.That(vm.PrintToFile).IsEqualTo(printToFile);
        await Assert.That(vm.PrintToPrinter).IsEqualTo(!printToFile);
        await Assert.That(vm.Printer).IsEqualTo("Printer");
        await Assert.That(vm.PrintFile).IsEqualTo("PrintFile");
    }

    [Test]
    public async Task Database_PropertyChanged_SavesUpdatedSettings()
    {
        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "Database Changed";
        vm.Database = NEW_VALUE;
        
        await Assert.That(_appSettings.Database).IsEqualTo(NEW_VALUE);
        _applicationSettingsRepository.Verify(s => s.Save(), Times.Once());
    }

    [Test]
    public async Task DymoPrinter_PropertyChanged_SavesUpdatedSettings()
    {
        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "Dymo Changed";
        vm.DymoPrinter = NEW_VALUE;

        await Assert.That(_appSettings.DymoPrinter).IsEqualTo(NEW_VALUE);
        _applicationSettingsRepository.Verify(s => s.Save(), Times.Once());
    }

    [Test]
    public async Task DymoPrinterFile_PropertyChanged_SavesUpdatedSettings()
    {
        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "DymoFile Changed";
        vm.DymoPrintFile = NEW_VALUE;

        await Assert.That(_appSettings.DymoPrintFile).IsEqualTo(NEW_VALUE);
        _applicationSettingsRepository.Verify(s => s.Save(), Times.Once());
    }

    [Test]
    public async Task Printer_PropertyChanged_SavesUpdatedSettings()
    {
//        mocker.Use(new UpdateManager("fake-path", locator: new TestVelopackLocator("my-app", "1.0.0", "packages")));
        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "Printer Changed";
        vm.Printer = NEW_VALUE;

        await Assert.That(_appSettings.Printer).IsEqualTo(NEW_VALUE);
        _applicationSettingsRepository.Verify(s => s.Save(), Times.Once());
    }

    [Test]
    public async Task PrintFile_PropertyChanged_SavesUpdatedSettings()
    {
        SettingsMainViewModel vm = _mocker.CreateInstance<SettingsMainViewModel>();

        const string NEW_VALUE = "PrintFile Changed";
        vm.PrintFile = NEW_VALUE;

        await Assert.That(_appSettings.PrintFile).IsEqualTo(NEW_VALUE);
        _applicationSettingsRepository.Verify(s => s.Save(), Times.Once());
    }
}