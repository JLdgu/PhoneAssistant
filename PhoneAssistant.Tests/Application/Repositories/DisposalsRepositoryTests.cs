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


    [Fact]
    public async Task GetSKU_ShouldBeCaseInsensitive()
    {
        List<StockKeepingUnit> mixedcase = [
            new StockKeepingUnit() { Manufacturer = "DeddeeE", Model = "BbCc", TrackedSKU = true },
            new StockKeepingUnit() { Manufacturer = "apple", Model = "iPhone se", TrackedSKU = true },
            new StockKeepingUnit() { Manufacturer = "APple", Model = "iphone 6s", TrackedSKU = true },
            new StockKeepingUnit() { Manufacturer = "Apple", Model = "Iphone 8S", TrackedSKU = true }
            ];
        _helper.DbContext.SKUs.AddRange(mixedcase);
        await _helper.DbContext.SaveChangesAsync();

        foreach (StockKeepingUnit sku in _helper.DbContext.SKUs.ToList())
        {
            StockKeepingUnit? upper = await _repository.GetSKUAsync(sku.Manufacturer.ToUpper(), sku.Model.ToUpper());
            StockKeepingUnit? lower = await _repository.GetSKUAsync(sku.Manufacturer.ToLower(), sku.Model.ToLower());

            Assert.Equal(sku, upper);
            Assert.Equal(sku, lower);
        }
    }

    #region MS Import
    [Fact]
    public async Task UpdateMSAsync_WithMissing_ThrowsException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.UpdateMSAsync("imei", "status"));
    }

    [Fact]
    public async Task UpdateMSAsync_WithModifiedExisting_UpdatesDisposal()
    {        
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusMS = "old status", Manufacturer = "OEM", Model = "model", TrackedSKU = true });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.UpdateMSAsync("imei", "new status");

        Assert.Equal(Result.Updated, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("new status", disposal.StatusMS);
    }

    [Fact]
    public async Task UpdateMSAsync_WithUnmodifiedExisting_ReturnsUnchanged()
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusMS = "status", Manufacturer = "OEM", Model = "model", TrackedSKU = true });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.UpdateMSAsync("imei", "status");

        Assert.Equal(Result.Unchanged, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusMS);
    }
    #endregion

    #region PA Import
    [Fact]
    public async Task UpdatePAAsync_WithMissing_ThrowsException()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.UpdatePAAsync("imei", "status",15));
    }

    [Theory]
    [InlineData("old", "new", 15, 15)]
    [InlineData("same", "same", 15, 20)]
    [InlineData("old", "new", 10, 10)]
    [InlineData("same", "same", 10, 20)]
    public async Task UpdatePAAsync_WithModified_UpdatesDisposal(string oldStatus, string newStatus, int oldSR, int newSR)
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusPA = oldStatus, SR = oldSR, Manufacturer = "OEM", Model = "model", TrackedSKU = true });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.UpdatePAAsync("imei", newStatus, newSR);

        Assert.Equal(Result.Updated, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal(newStatus, disposal.StatusPA);
        Assert.Equal(newSR,disposal.SR);
    }

    [Fact]
    public async Task UpdatePAAsync_WithUnmodified_ReturnsUnchanged()
    {
        await _helper.DbContext.Disposals.AddAsync(new Disposal() { Imei = "imei", StatusPA = "status", SR = 25, Manufacturer = "OEM", Model = "model", TrackedSKU = true });
        await _helper.DbContext.SaveChangesAsync();

        Result result = await _repository.UpdatePAAsync("imei", "status", 25);

        Assert.Equal(Result.Unchanged, result);
        Disposal? disposal = await _helper.DbContext.Disposals.FindAsync("imei");
        Assert.NotNull(disposal);
        Assert.Equal("imei", disposal.Imei);
        Assert.Equal("status", disposal.StatusPA);
        Assert.Equal(25, disposal.SR);
    }
    #endregion
}
