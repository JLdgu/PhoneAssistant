using System.Security.Policy;

using Moq;
using Moq.AutoMock;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;
using PhoneAssistant.WPF.Features.Disposals;

using Xunit;
using Xunit.Abstractions;

namespace PhoneAssistant.Tests.Features.Disposals;

public sealed class DisposalMainViewModelTests
{
    [Fact]
    public async Task LoadAsync_ShouldSetLatestImportNone_WhenPreviousImport()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();
        Mock<IImportHistoryRepository> history = _mocker.GetMock<IImportHistoryRepository>();
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalMS)).ReturnsAsync((ImportHistory)null);
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalPA)).ReturnsAsync((ImportHistory)null);
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalSCC)).ReturnsAsync((ImportHistory)null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

        await sut.LoadAsync();

        Assert.Equal("Latest Import: None", sut.LatestMSImport);
        Assert.Equal("Latest Import: None", sut.LatestPAImport);
        Assert.Equal("Latest Import: None", sut.LatestSCCImport);
        history.VerifyAll();
    }
    [Fact]
    public async Task LoadAsync_ShouldSetLatestImportDate_WhenPreviousImport()
    {
        AutoMocker _mocker = new AutoMocker();
        DisposalsMainViewModel sut = _mocker.CreateInstance<DisposalsMainViewModel>();
        Mock<IImportHistoryRepository> history = _mocker.GetMock<IImportHistoryRepository>();
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalMS)).ReturnsAsync(new ImportHistory() { Name = ImportType.DisposalMS, File = "File", ImportDate = "MS" });
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalPA)).ReturnsAsync(new ImportHistory() { Name = ImportType.DisposalMS, File = "File", ImportDate = "PA" });
        history.Setup(h => h.GetLatestImportAsync(ImportType.DisposalSCC)).ReturnsAsync(new ImportHistory() { Name = ImportType.DisposalMS, File = "File", ImportDate = "SCC" });

        await sut.LoadAsync();

        Assert.Equal("Latest Import: File (MS)", sut.LatestMSImport);
        Assert.Equal("Latest Import: File (PA)", sut.LatestPAImport);
        Assert.Equal("Latest Import: File (SCC)", sut.LatestSCCImport);
        history.VerifyAll();
    }
}
