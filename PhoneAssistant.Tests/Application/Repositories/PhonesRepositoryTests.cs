using System.ComponentModel;

using Microsoft.EntityFrameworkCore;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using Xunit;

namespace PhoneAssistant.Tests.Application.Repositories;

public sealed class PhonesRepositoryTests : DbTestHelper
{
    private readonly ITestOutputHelper _output;
    readonly DbTestHelper _helper = new();
    readonly PhonesRepository _repository;

    const string ASSET_TAG = "asset";
    const string CONDITION_N = "N";
    const string CONDITION_R = "R";
    const string DESPATCH_DETAILS = "Despatch";
    const string FORMER_USER = "former user";
    const string IMEI = "imei";
    const string MODEL = "model";
    const string NEW_USER = "new user";
    const string NOTES = "notes";
    const string PHONE_NUMBER = "phone number";
    const string SIM_NUMBER = "sim number";
    const int SR = 12345;
    const string STATUS = "Production";

    readonly Phone _phone = new()
    {
        AssetTag = ASSET_TAG,
        Condition = CONDITION_R,
        DespatchDetails = DESPATCH_DETAILS,
        FormerUser = FORMER_USER,
        Imei = IMEI,
        Model = MODEL,
        NewUser = NEW_USER,
        Notes = NOTES,
        OEM = OEMs.Nokia,
        PhoneNumber = PHONE_NUMBER,
        SimNumber = SIM_NUMBER,
        SR = SR,
        Status = STATUS
    };

    public PhonesRepositoryTests(ITestOutputHelper output)
    {
        _output = output;
        _repository = new(_helper.DbContext);
    }

    [Fact]
    async Task AssetTagUnique_ShouldReturnTrue_WhenAssetTagNull()
    {
        bool actual = await _repository.AssetTagUniqueAsync(null);

        Assert.True(actual);
    }

    [Fact]
    async Task AssetTagUnique_ShouldReturnTrue_WhenPhoneDoesNotExist()
    {
        bool actual = await _repository.AssetTagUniqueAsync("DoesNotExist");

        Assert.True(actual);
    }

    [Fact]
    async Task AssetTagUnique_ShouldReturnFalse_WhenPhoneDoesExistAsync()
    {
        _helper.DbContext.Phones.Add(_phone);
        await _helper.DbContext.SaveChangesAsync();

        bool actual = await _repository.AssetTagUniqueAsync(_phone.AssetTag);

        Assert.False(actual);
    }

    [Fact]
    async Task Exists_ShouldReturnFalse_WhenPhoneDoesNotExist()
    {
        bool actual = await _repository.ExistsAsync("DoesNotExist");

        Assert.False(actual);
    }

    [Fact]
    async Task Exists_ShouldReturnTrue_WhenPhoneDoesExistAsync()
    {
        _helper.DbContext.Phones.Add(_phone);
        await _helper.DbContext.SaveChangesAsync();

        bool actual = await _repository.ExistsAsync(_phone.Imei);

        Assert.True(actual);
    }

    [Fact]
    public async Task UpdateStatusAsync_WithNullImei_ThrowsException()
    {
#pragma warning disable CS8604 // Converting null literal or possible null value to non-nullable type.
        string? imei = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateStatusAsync(imei, "ApplicationSettings.StatusDisposed"));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateStatusAsync_WithNullStatus_ThrowsException()
    {
#pragma warning disable CS8604 // Converting null literal or possible null value to non-nullable type.
        string? status = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateStatusAsync("imei", status));
#pragma warning restore CS8604 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateStatusAsync_WithPhoneNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateStatusAsync("not found", ApplicationConstants.StatusDisposed));
    }

    [Fact]
    public async Task UpdateStatusAsync_WithStatusChange_Succeeds()
    {
        _phone.Status = ApplicationConstants.StatusDecommissioned;
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();

        await _repository.UpdateStatusAsync(_phone.Imei, ApplicationConstants.StatusDisposed);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(_phone.Imei);
        Assert.NotNull(actual);
        Assert.Equal(ApplicationConstants.StatusDisposed,actual.Status);
    }

    [Fact]
    public async Task UpdateAsync_WithNullPhone_ThrowsException()
    {
#pragma warning disable CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
        Phone phone = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateAsync(phone));
#pragma warning restore CS8600, CS8604 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateAsync_WithPhoneNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(_phone));
    }

    [Fact]
    public async Task UpdateAsync_WithPhoneFound_Succeeds()
    {
        Phone original = new()
        {
            Imei = _phone.Imei,
            Condition = CONDITION_N,
            Model = "old model",
            OEM = OEMs.Apple,
            Status = ApplicationConstants.Statuses[1]
        };
        await _helper.DbContext.Phones.AddAsync(original, TestContext.Current.CancellationToken);
        await _helper.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        string lastUpdate = original.LastUpdate;

        await _repository.UpdateAsync(_phone);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(new object?[] { _phone.Imei }, TestContext.Current.CancellationToken);
        Assert.NotNull(actual);
        Assert.Equal(_phone.AssetTag, actual.AssetTag);
        Assert.Equal(_phone.Condition, actual.Condition);
        Assert.Equal(_phone.DespatchDetails, actual.DespatchDetails);
        Assert.Equal(_phone.FormerUser, actual.FormerUser);
        Assert.Equal(_phone.Imei, actual.Imei);
        Assert.Equal(_phone.Model, actual.Model);
        Assert.Equal(_phone.NewUser, actual.NewUser);
        Assert.Equal(_phone.Notes, actual.Notes);
        Assert.Equal(_phone.OEM, actual.OEM);
        Assert.Equal(_phone.PhoneNumber, actual.PhoneNumber);
        Assert.Equal(_phone.SimNumber, actual.SimNumber);
        Assert.Equal(_phone.SR, actual.SR);
        Assert.Equal(_phone.Status, actual.Status);

        UpdateHistoryPhone? history = await _helper.DbContext.UpdateHistoryPhones.FirstOrDefaultAsync(h => h.Id > 0, cancellationToken: TestContext.Current.CancellationToken);
        Assert.NotNull(history);
        Assert.Equal(history.AssetTag, actual.AssetTag);
        Assert.Equal(history.Condition, actual.Condition);
        Assert.Equal(history.DespatchDetails, actual.DespatchDetails);
        Assert.Equal(history.FormerUser, actual.FormerUser);
        Assert.Equal(history.Imei, actual.Imei);
        Assert.Equal(history.Model, actual.Model);
        Assert.Equal(history.NewUser, actual.NewUser);
        Assert.Equal(history.Notes, actual.Notes);
        Assert.Equal(history.OEM, actual.OEM);
        Assert.Equal(history.PhoneNumber, actual.PhoneNumber);
        Assert.Equal(history.SimNumber, actual.SimNumber);
        Assert.Equal(history.SR, actual.SR);
        Assert.Equal(history.Status, actual.Status);
    }

    [Fact]
    public async Task UpdateAsync_WithDuplicateUpdate()
    {
        await _helper.DbContext.Phones.AddAsync(
            new Phone()
            {
                Imei = _phone.Imei,
                Condition = CONDITION_N,
                Model = "old model",
                OEM = OEMs.Apple,
                Status = ApplicationConstants.Statuses[1]
            });
        await _helper.DbContext.SaveChangesAsync();

        await _repository.UpdateAsync(_phone);

        await _repository.UpdateAsync(_phone);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(_phone.Imei);

        UpdateHistoryPhone? history = await _helper.DbContext.UpdateHistoryPhones.FindAsync(1);
        Assert.NotNull(history);

        history = await _helper.DbContext.UpdateHistoryPhones.FindAsync(2);
        Assert.Null(history);
    }

    //    [Fact]
    //    public async Task UpdateKeyAsync_WithNullOldImei_ThrowsException()
    //    {
    //#pragma warning disable CS8625 // Converting null literal or possible null value to non-nullable type.
    //        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateKeyAsync(null, "new"));
    //#pragma warning restore CS8625 // Possible null reference argument.
    //    }

    //    [Fact]
    //    public async Task UpdateKeyAsync_WithNullNewImei_ThrowsException()
    //    {
    //#pragma warning disable CS8625 // Converting null literal or possible null value to non-nullable type.
    //        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateKeyAsync("old", null));
    //#pragma warning restore CS8625 // Possible null reference argument.
    //    }

    //    [Fact]
    //    public async Task UpdateKeyAsync_WithPhoneNotFound_ThrowsException()
    //    {
    //        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(_phone));
    //    }

    //    [Fact]
    //    public async Task UpdateKeyAsync_WithPhoneFound_Succeeds()
    //    {
    //        const string OLD_IMEI = "old IMEI";
    //        _phone.Imei = OLD_IMEI;
    //        await _helper.DbContext.Phones.AddAsync(_phone);
    //        await _helper.DbContext.SaveChangesAsync();
    //        const string NEW_IMEI = "new IMEI";

    //        string lastUpdate = await _repository.UpdateKeyAsync(OLD_IMEI, NEW_IMEI);

    //        Phone? removed = await _helper.DbContext.Phones.FindAsync(OLD_IMEI);
    //        Assert.Null(removed);
    //        Phone? actual = await _helper.DbContext.Phones.FindAsync(NEW_IMEI);
    //        Assert.NotNull(actual);
    //        Assert.Equal(NEW_IMEI, actual.Imei);
    //        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", lastUpdate);
    //    }
}
