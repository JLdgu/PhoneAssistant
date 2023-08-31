using System;

using Moq;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Features.Settings;

namespace PhoneAssistant.Tests.Features.Settings;

[TestClass]
public sealed class SettingsMainViewModelTests
{
    [TestMethod]
    public void Constructor_SetsVMProperties()
    {
        const string VERSION = "0.0.0.1";
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.AssemblyVersion).Returns(new Version(VERSION));
        mock.Setup(s => s.PrintToFile).Returns(false);
        mock.Setup(s => s.Printer).Returns("Printer");
        mock.Setup(s => s.PrintFile).Returns("PrintFile");
        var vm = new SettingsMainViewModel(mock.Object);

        Assert.IsTrue(vm.PrintToPrinter);
        Assert.IsFalse(vm.PrintToFile);
        Assert.AreEqual("Printer", vm.Printer);
        Assert.AreEqual("PrintFile", vm.PrintFile);

        Assert.AreEqual(VERSION, vm.VersionDescription);

        mock.VerifyGet(s => s.AssemblyVersion);
    }

    [TestMethod]
    public void Constructor_SettingPrintToFileFalse_SetsPrintToPrinterTrue()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.PrintToFile).Returns(false);

        var vm = new SettingsMainViewModel(mock.Object);

        Assert.IsTrue(vm.PrintToPrinter);
        Assert.IsFalse(vm.PrintToFile);
    }

    [TestMethod]
    public void Constructor_SettingPrintToFileTrue_SetsPrintToFileTrue()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.PrintToFile).Returns(true);

        var vm = new SettingsMainViewModel(mock.Object);

        Assert.IsTrue(vm.PrintToFile);
        Assert.IsFalse(vm.PrintToPrinter);
    }
    
    [TestMethod]
    public void Database_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.Database).Returns("Database");              
        var vm = new SettingsMainViewModel(mock.Object);
        const string NEW_VALUE = "Database Changed";

        vm.Database = NEW_VALUE;

        mock.VerifySet(s => s.Database = NEW_VALUE);
        mock.Verify(s => s.Save(), Times.Once());
    }

    [TestMethod]
    public void Printer_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.Printer).Returns("Printer");
        var vm = new SettingsMainViewModel(mock.Object);
        const string NEW_VALUE = "Printer Changed";

        vm.Printer = NEW_VALUE;

        mock.VerifySet(s => s.Printer = NEW_VALUE);
        mock.Verify(s => s.Save(), Times.Once());
    }

    [TestMethod]
    public void PrinterFile_PropertyChanged_SavesUpdatedSettings()
    {
        var mock = new Mock<IUserSettings>();
        mock.Setup(s => s.PrintFile).Returns("PrintFile");
        var vm = new SettingsMainViewModel(mock.Object);
        const string NEW_VALUE = "PrintFile Changed";

        vm.PrintFile = NEW_VALUE;

        mock.VerifySet(s => s.PrintFile = NEW_VALUE);
        mock.Verify(s => s.Save(), Times.Once());
    }
}
