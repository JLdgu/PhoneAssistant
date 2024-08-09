using CommunityToolkit.Mvvm.Messaging;

using Moq;

using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;
public class ImportSCCTests
{
    private readonly Mock<IDisposalsRepository> _disposal = new();
    private readonly Mock<IMessenger> _messenger = new();
    private readonly XSSFWorkbook _workbook = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private ImportSCC _sut;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    [Fact]
    public async Task ParseRow_ShouldSetAction_WhenRowNull()
    {
        ImportSCC sut = new("", _disposal.Object, _messenger.Object);
        IRow? row = null;

#pragma warning disable CS8604 // Possible null reference argument.
        Disposal actual = await sut.ParseRowAsync(row);
#pragma warning restore CS8604 // Possible null reference argument.

        Assert.Equal(ReconciliationConstants.ImeiInvalid, actual.Action);
    }

    [Fact]
    public async Task ParseRow_ShouldSetAction_WhenSerialNumberCellTypeFormula()
    {
        IRow row = TestSetup();        
        ICell cell = row.GetCell(ImportSCC.SerialNumber);
        cell.SetCellFormula("SUM(1+2)");

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("SUM(1+2)", actual.Imei);
        Assert.NotNull(actual.Action);
    }

    [Fact]
    public async Task ParseRow_ShouldSetAction_WhenSerialNumberStringCellWithStringValue()
    {
        IRow row = TestSetup();
        ICell cell = row.GetCell(ImportSCC.SerialNumber);
        cell.SetCellValue("ABC");

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("ABC", actual.Imei);
        Assert.NotNull(actual.Action);
    }

    [Fact]
    public async Task ParseRow_ShouldSetAction_WhenSKUDoesNotExist()
    {
        IRow row = TestSetup();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        _disposal.Setup(r => r.GetSKUAsync("oem", "model")).ReturnsAsync((StockKeepingUnit)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("oem", actual.Manufacturer);
        Assert.Equal("model", actual.Model);
        Assert.False(actual.TrackedSKU);
        Assert.Equal(ReconciliationConstants.UnknownSKU, actual.Action);
        _disposal.Verify();
    }

    [Fact]
    public async Task ParseRow_ShouldSetAction_WhenStatusCellDoesNotContainsDespatched()
    {
        IRow row = TestSetup();
        ICell cell = row.GetCell(ImportSCC.Status);
        cell.SetCellValue("*despaaaaatched*");

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal(ReconciliationConstants.CheckSCCStatus, actual.Action);
    }

    [Fact]
    public async Task ParseRow_ShouldSetCertificate_WhenTrackerIdNumericCell()
    {
        IRow row = TestSetup();
        ICell cell = row.GetCell(ImportSCC.TrackerId);
        cell.SetCellValue(3579);

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal(3579, actual.Certificate);
    }

    [Fact]
    public async Task ParseRow_ShouldSetDefaultCertificate_WhenTrackerIdNotNumericCell()
    {
        IRow row = TestSetup();
        ICell cell = row.GetCell(ImportSCC.TrackerId);
        cell.SetCellValue("aaa");

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal(0, actual.Certificate);
    }

    [Fact]
    public async Task ParseRow_ShouldSetImei_WhenSerialNumberNumericCell()
    {
        _disposal.Setup(r => r.GetSKUAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new StockKeepingUnit() { Manufacturer = "Maunfacturer", Model = "Model", TrackedSKU = true });
        IRow row = TestSetup();
        ICell cell = row.GetCell(ImportSCC.SerialNumber);
        cell.SetCellValue(1);

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("000000000000001", actual.Imei);
        Assert.Null(actual.Action);
    }

    [Fact]
    public async Task ParseRow_ShouldSetImei_WhenSerialNumberStringCellWithNumericValue()
    {
        IRow row = TestSetup();
        _disposal.Setup(r => r.GetSKUAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new StockKeepingUnit() { Manufacturer = "OEM", Model = "Model", TrackedSKU = true });
        ICell cell = row.GetCell(ImportSCC.SerialNumber);
        cell.SetCellValue("12");

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("000000000000012", actual.Imei);
        Assert.Null(actual.Action);
    }

    [Fact]
    public async Task ParseRow_ShouldSetSKU_WhenSKUExists()
    {
        _disposal.Setup(r => r.GetSKUAsync("maunfacturer", "design")).ReturnsAsync(new StockKeepingUnit() { Manufacturer = "Maunfacturer", Model = "Design", TrackedSKU = true });
        IRow row = TestSetup();
        ICell cell = row.CreateCell(ImportSCC.Manufacturer);
        cell.SetCellValue("maunfacturer");
        cell = row.CreateCell(ImportSCC.Model);
        cell.SetCellValue("design");

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("Maunfacturer", actual.Manufacturer);
        Assert.Equal("Design", actual.Model);
        Assert.True(actual.TrackedSKU);
        Assert.Null(actual.Action);
        _disposal.Verify();
    }

    [Fact]
    public async Task ParseRow_ShouldSetSKUModelUnknown_WhenSKUModelMatchesCertificate()
    {
        IRow row = TestSetup();
        _disposal.Setup(r => r.GetSKUAsync("maunfacturer", "Unknown")).ReturnsAsync(new StockKeepingUnit() { Manufacturer = "Maunfacturer", Model = "Unknown", TrackedSKU = false });
        ICell cell = row.CreateCell(ImportSCC.Manufacturer);
        cell.SetCellValue("maunfacturer");
        cell = row.GetCell(ImportSCC.Model);
        cell.SetCellValue(123456);

        Disposal actual = await _sut.ParseRowAsync(row);

        Assert.NotNull(actual);
        Assert.Equal("Unknown", actual.Model);
        Assert.Null(actual.Action);
        _disposal.Verify();
    }

    private IRow TestSetup()
    {
        _sut = new("", _disposal.Object, _messenger.Object);

        IRow row = _workbook.CreateSheet("Sheet1").CreateRow(0);
        ICell cell = row.CreateCell(ImportSCC.TrackerId);
        cell.SetCellValue(123456);
        cell = row.CreateCell(ImportSCC.SerialNumber);
        cell.SetCellValue(1);
        cell = row.CreateCell(ImportSCC.Status);
        cell.SetCellValue("*despatched*");
        cell = row.CreateCell(ImportSCC.Manufacturer);
        cell.SetCellValue("oem");
        cell = row.CreateCell(ImportSCC.Model);
        cell.SetCellValue("model");
        _workbook.Close();
        return row;
    }
}
