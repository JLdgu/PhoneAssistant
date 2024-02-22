using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using Xunit;

namespace PhoneAssistant.Tests.Application.Repositories;

public class DisposalsRepositoryTests
{
    readonly DbTestHelper _helper = new();
    readonly DisposalsRepository _repository;

    public DisposalsRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    #region MS Import
    [Fact]
    public async Task AddOrUpdateMSAsync_WithNew_AddsDisposal()
    {
        Result result = await _repository.AddOrUpdateMSAsync("imei", "status");

        Assert.Equal(Result.Added, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusMS);
    }

    [Fact]
    public async Task AddOrUpdateMSAsync_WithModifiedExisting_UpdatesDisposal()
    {        
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusMS = "old status" });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.AddOrUpdateMSAsync("imei", "new status");

        Assert.Equal(Result.Updated, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("new status", disposal.StatusMS);
    }

    [Fact]
    public async Task AddOrUpdateMSAsync_WithUnmodifiedExisting_ReturnsUnchanged()
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusMS = "status" });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.AddOrUpdateMSAsync("imei", "status");

        Assert.Equal(Result.Unchanged, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusMS);
    }
    #endregion

    #region PA Import
    [Fact]
    public async Task AddOrUpdatePAAsync_WithNew_AddsDisposal()
    {
        Result result = await _repository.AddOrUpdatePAAsync("imei", "status",15);

        Assert.Equal(Result.Added, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusPA);
        Assert.Equal(15, disposal.SR);
    }

    [Theory]
    [InlineData("old", "new", null,15)]
    [InlineData("same", "same", null, 15)]
    [InlineData("old", "new", 10, 20)]
    [InlineData("same", "same", 10, 20)]
    public async Task AddOrUpdatePAAsync_WithModifiedExisting_UpdatesDisposal(string oldStatus, string newStatus, int? oldSR, int newSR)
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusPA = oldStatus, SR = oldSR });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.AddOrUpdatePAAsync("imei", newStatus, newSR);

        Assert.Equal(Result.Updated, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal(newStatus, disposal.StatusPA);
        Assert.Equal(newSR,disposal.SR);
    }

    [Fact]
    public async Task AddOrUpdatePAAsync_WithUnmodifiedExisting_ReturnsUnchanged()
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusPA = "status" , SR = 25});
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.AddOrUpdatePAAsync("imei", "status", 25);

        Assert.Equal(Result.Unchanged, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusPA);
        Assert.Equal(25, disposal.SR);
    }
    #endregion

    #region SCC Import
    [Theory]
    [InlineData(null)]
    [InlineData(100)]
    public async Task AddOrUpdateSCCAsync_WithNew_AddsDisposal(int? certificate)
    {
        Result result = await _repository.AddOrUpdateSCCAsync("imei", "status", certificate);

        Assert.Equal(Result.Added, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusSCC);
        Assert.Equal(certificate, disposal.Certificate);
    }

    [Theory]
    [InlineData("old", "new", null, 115)]
    [InlineData("same", "same", null, 115)]
    [InlineData("old", "new", 110, 120)]
    [InlineData("same", "same", 110, 120)]
    public async Task AddOrUpdateSCCAsync_WithModifiedExisting_UpdatesDisposal(string oldStatus, string newStatus, int? oldCertificate, int newCertificate)
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusSCC = oldStatus, Certificate = oldCertificate });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.AddOrUpdateSCCAsync("imei", newStatus, newCertificate);

        Assert.Equal(Result.Updated, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal(newStatus, disposal.StatusSCC);
        Assert.Equal(newCertificate, disposal.Certificate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(125)]
    public async Task AddOrUpdateSCCAsync_WithUnmodifiedExisting_ReturnsUnchanged(int? certificate)
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusSCC = "status", Certificate = certificate });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.AddOrUpdateSCCAsync("imei", "status", certificate);

        Assert.Equal(Result.Unchanged, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusSCC);
        Assert.Equal(certificate, disposal.Certificate);
    }
    #endregion
}
