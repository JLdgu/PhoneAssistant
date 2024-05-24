using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class DisposalMainViewModelTests()
{
    [Fact]
    public async Task LoadAsync_ShouldSetLatestNone_WhenNoLatestExists()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();
        Mock<IImportHistoryRepository> history = _mocker.GetMock<IImportHistoryRepository>();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalMS)).ReturnsAsync((ImportHistory)null);
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalPA)).ReturnsAsync((ImportHistory)null);
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalSCC)).ReturnsAsync((ImportHistory)null);
        history.Setup(h => h.GetLatestImportAsync(ImportType.Reconiliation)).ReturnsAsync((ImportHistory)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        await sut.LoadAsync();

        Assert.Equal("Latest Import: None", sut.LatestMSImport);
        Assert.Equal("Latest Import: None", sut.LatestPAImport);
        Assert.Equal("Latest Import: None", sut.LatestSCCImport);
        Assert.Equal("Latest Reconiliation: None", sut.LatestReconiliation);
        history.VerifyAll();
    }
    
    [Fact]
    public async Task LoadAsync_ShouldSetLatestDate_WhenLatestExists()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();
        Mock<IImportHistoryRepository> history = _mocker.GetMock<IImportHistoryRepository>();
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalMS)).ReturnsAsync(new ImportHistory() { Name = ImportType.DisposalMS, File = "File", ImportDate = "MS" });
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalPA)).ReturnsAsync(new ImportHistory() { Name = ImportType.DisposalMS, File = "File", ImportDate = "PA" });
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalSCC)).ReturnsAsync(new ImportHistory() { Name = ImportType.DisposalMS, File = "File", ImportDate = "SCC" });
        history.Setup(h => h.GetLatestImportAsync(ImportType.Reconiliation)).ReturnsAsync(new ImportHistory() { Name = ImportType.Reconiliation, File = "", ImportDate = "Reconcile" });

        await sut.LoadAsync();

        Assert.Equal("Latest Import: File (MS)", sut.LatestMSImport);
        Assert.Equal("Latest Import: File (PA)", sut.LatestPAImport);
        Assert.Equal("Latest Import: File (SCC)", sut.LatestSCCImport);
        Assert.Equal("Latest Reconiliation: Reconcile", sut.LatestReconiliation);
        history.VerifyAll();
    }

    [Fact]
    public void Receive_ShouldSetMSMaxProgress_WhenMessageTypeMSMaxProgress()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.MSMaxProgress, "", 257));

        Assert.Equal(257, sut.MSMaxProgress);
        Assert.Single(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldSetMSProgress_WhenMessageTypeMSProgress()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.MSProgress, "", 157));

        Assert.Equal(157, sut.MSProgress);
        Assert.Empty(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldSetPAMaxProgress_WhenMessageTypePAMaxProgress()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.PAMaxProgress, "", 3257));

        Assert.Equal(3257, sut.PAMaxProgress);
        Assert.Single(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldSetPAProgress_WhenMessageTypePAProgress()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.PAProgress, "", 1547));

        Assert.Equal(1547, sut.PAProgress);
        Assert.Empty(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldSetSCCMaxProgress_WhenMessageTypeSCCMaxProgress()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.SCCMaxProgress, "", 57));

        Assert.Equal(57, sut.SCCMaxProgress);
        Assert.Single(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldSetSCCProgress_WhenMessageTypeSCCProgress()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.SCCProgress, "", 457));

        Assert.Equal(457, sut.SCCProgress);
        Assert.Empty(sut.LogItems);
    }

    [Fact]
    public void Receive_ShouldUpdateLogItems_WhenMessageTypeDefault()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();

        sut.Receive(new LogMessage(MessageType.Default, "Some message"));

        Assert.Single( sut.LogItems);
    }
}
