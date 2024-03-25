using System.ComponentModel;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Application.Repositories;

using Xunit;

namespace PhoneAssistant.Tests.Application.Repositories;

public sealed class PhonesRepositoryTests : DbTestHelper
{
    readonly DbTestHelper _helper = new();
    readonly PhonesRepository _repository;
    readonly Phone _phone = new()
    {
        Imei = "imei",
        Model = "model",
        NorR = "R",
        OEM = "Nokia",
        PhoneNumber = "phoneNumber",
        SimNumber = "simNumber",
        Status = "status"
    };

    public PhonesRepositoryTests()
    {
        _repository = new(_helper.DbContext);
    }

    [Fact]
    public async Task RemoveSimFromPhone_WithNullPhone_ThrowsException()
    {
#pragma warning disable CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
        Phone phone = null;
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.RemoveSimFromPhone(phone));
#pragma warning restore CS8600, CS8604 // Converting null literal or possible null value to non-nullable type.
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RemoveSimFromPhone_WithNullOrEmptyPhoneNumber_ThrowsException(string? phoneNumber)
    {
        _phone.PhoneNumber = phoneNumber;

        await Assert.ThrowsAsync<ArgumentException>(() => _repository.RemoveSimFromPhone(_phone));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task RemoveSimFromPhone_WithNullOrEmptySimNumber_ThrowsException(string? simNumber)
    {
        _phone.SimNumber = simNumber;

        await Assert.ThrowsAsync<ArgumentException>(() => _repository.RemoveSimFromPhone(_phone));
    }

    [Fact]
    public async Task RemoveSimFromPhone_WithMissingPhone_ThrowsException()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.RemoveSimFromPhone(_phone));
    }

    [Fact]
    [Description("Issue 21")]
    public async Task RemoveSimFromPhone_WithExistingSim_Succeeds()
    {
        const string PHONE_NUMBER = "phone number";
        const string SIM_NUMBER = "sim number";
        _phone.PhoneNumber = PHONE_NUMBER;
        _phone.SimNumber = SIM_NUMBER;
        await _helper.DbContext.Phones.AddAsync(_phone);
        Sim? sim = new Sim() { PhoneNumber = PHONE_NUMBER, SimNumber = SIM_NUMBER };
        await _helper.DbContext.Sims.AddAsync(sim);
        await _helper.DbContext.SaveChangesAsync();

        Phone updatedPhone = await _repository.RemoveSimFromPhone(_phone);

        Assert.Null(updatedPhone.PhoneNumber);
        Assert.Null(updatedPhone.SimNumber);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", updatedPhone.LastUpdate);

        Sim? dbSim = await _helper.DbContext.Sims.FindAsync(PHONE_NUMBER);
        Assert.NotNull(dbSim);
        Assert.Equal(PHONE_NUMBER, dbSim.PhoneNumber);
        Assert.Equal(SIM_NUMBER, dbSim.SimNumber);
        Assert.Equal("In Stock", dbSim.Status);

        Phone? dbPhone = await _helper.DbContext.Phones.FindAsync("imei");
        Assert.NotNull(dbPhone);
        Assert.Null(dbPhone.PhoneNumber);
        Assert.Null(dbPhone.SimNumber);
    }

    [Fact]
    public async Task RemoveSimFromPhone_WithNewSim_Succeeds()
    {
        const string PHONE_NUMBER = "phone number";
        const string SIM_NUMBER = "sim number";
        _phone.PhoneNumber = PHONE_NUMBER;
        _phone.SimNumber = SIM_NUMBER;
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();

        Phone updatedPhone = await _repository.RemoveSimFromPhone(_phone);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", updatedPhone.LastUpdate);

        Sim? sim = await _helper.DbContext.Sims.FindAsync(PHONE_NUMBER);
        Assert.NotNull(sim);
        Assert.Equal(PHONE_NUMBER, sim.PhoneNumber);
        Assert.Equal(SIM_NUMBER, sim.SimNumber);
        Assert.Equal("In Stock", sim.Status);

        var phone = await _helper.DbContext.Phones.FindAsync("imei");
        Assert.NotNull(phone);
        Assert.Null(phone.PhoneNumber);
        Assert.Null(phone.SimNumber);
    }

    [Fact]
    [Description("Issue 19")]
    public async Task RemoveSimFromPhone_OnlyChangesPhoneandSimNumberFields()
    {
        const string EXPECTED_IMEI = "imei";
        const string MOVE_PHONE_NUMBER = "phone number";
        const string MOVE_SIM_NUMBER = "sim number";
        const string EXPECTED_ASSET_TAG = "asset";
        const string EXPECTED_FORMER_USER = "former user";
        const string EXPECTED_MODEL = "model";
        const string EXPECTED_NEW_USER = "new user";
        const string EXPECTED_NORR = "R";
        const string EXPECTED_NOTES = "notes";
        const string EXPECTED_OEM = "Samsung";
        const int EXPECTED_SR = 12345;
        const string EXPECTED_STATUS = "status";
        Phone? phone = new()
        {
            Imei = EXPECTED_IMEI,
            PhoneNumber = MOVE_PHONE_NUMBER,
            SimNumber = MOVE_SIM_NUMBER,
            AssetTag = EXPECTED_ASSET_TAG,
            FormerUser = EXPECTED_FORMER_USER,
            Model = EXPECTED_MODEL,
            NewUser = EXPECTED_NEW_USER,
            NorR = EXPECTED_NORR,
            Notes = EXPECTED_NOTES,
            OEM = EXPECTED_OEM,
            SR = EXPECTED_SR,
            Status = EXPECTED_STATUS
        };
        await _helper.DbContext.Phones.AddAsync(phone);
        await _helper.DbContext.SaveChangesAsync();

        Phone updatedPhone = await _repository.RemoveSimFromPhone(phone);

        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", updatedPhone.LastUpdate);
        Sim? sim = await _helper.DbContext.Sims.FindAsync(MOVE_PHONE_NUMBER);
        Assert.NotNull(sim);
        Assert.Equal(MOVE_PHONE_NUMBER, sim.PhoneNumber);
        Assert.Equal(MOVE_SIM_NUMBER, sim.SimNumber);
        Assert.Equal("In Stock", sim.Status);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(EXPECTED_IMEI);
        Assert.NotNull(actual);
        Assert.Null(actual.PhoneNumber);
        Assert.Null(actual.SimNumber);
        Assert.Equal(EXPECTED_ASSET_TAG, actual.AssetTag);
        Assert.Equal(EXPECTED_FORMER_USER, actual.FormerUser);
        Assert.Equal(EXPECTED_MODEL, actual.Model);
        Assert.Equal(EXPECTED_NEW_USER, actual.NewUser);
        Assert.Equal(EXPECTED_NORR, actual.NorR);
        Assert.Equal(EXPECTED_NOTES, actual.Notes);
        Assert.Equal(EXPECTED_OEM, actual.OEM);
        Assert.Equal(EXPECTED_SR, actual.SR);
        Assert.Equal(EXPECTED_STATUS, actual.Status);
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
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();
        Phone? expected = new()
        {
            Imei = _phone.Imei,
            DespatchDetails = "despatch",
            Model = "model2",
            NorR = "N",
            OEM = "Apple",
            PhoneNumber = "phone2",
            SimNumber = "sim2",
            Status = "status2"
        };

        string lastUpdate = await _repository.UpdateAsync(expected);

        Phone? actual = await _helper.DbContext.Phones.FindAsync(_phone.Imei);
        Assert.NotNull(actual);
        Assert.Equal(expected.AssetTag, actual.AssetTag);
        Assert.Equal(expected.DespatchDetails, actual.DespatchDetails);
        Assert.Equal(expected.FormerUser, actual.FormerUser);
        Assert.Equal(expected.Imei, actual.Imei);
        Assert.Equal(expected.Model, actual.Model);
        Assert.Equal(expected.NewUser, actual.NewUser);
        Assert.Equal(expected.NorR, actual.NorR);
        Assert.Equal(expected.Notes, actual.Notes);
        Assert.Equal(expected.OEM, actual.OEM);
        Assert.Equal(expected.PhoneNumber, actual.PhoneNumber);
        Assert.Equal(expected.SimNumber, actual.SimNumber);
        Assert.Equal(expected.SR, actual.SR);
        Assert.Equal(expected.Status, actual.Status);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", lastUpdate);
    }

    [Fact]
    public async Task UpdateKeyAsync_WithNullOldImei_ThrowsException()
    {
#pragma warning disable CS8625 // Converting null literal or possible null value to non-nullable type.
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateKeyAsync(null, "new"));
#pragma warning restore CS8625 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateKeyAsync_WithNullNewImei_ThrowsException()
    {
#pragma warning disable CS8625 // Converting null literal or possible null value to non-nullable type.
        await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.UpdateKeyAsync("old", null));
#pragma warning restore CS8625 // Possible null reference argument.
    }

    [Fact]
    public async Task UpdateKeyAsync_WithPhoneNotFound_ThrowsException()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => _repository.UpdateAsync(_phone));
    }

    [Fact]
    public async Task UpdateKeyAsync_WithPhoneFound_Succeeds()
    {
        const string OLD_IMEI = "old IMEI";
        _phone.Imei = OLD_IMEI;
        await _helper.DbContext.Phones.AddAsync(_phone);
        await _helper.DbContext.SaveChangesAsync();
        const string NEW_IMEI = "new IMEI";

        string lastUpdate = await _repository.UpdateKeyAsync(OLD_IMEI, NEW_IMEI);

        Phone? removed = await _helper.DbContext.Phones.FindAsync(OLD_IMEI);
        Assert.Null(removed);
        Phone? actual = await _helper.DbContext.Phones.FindAsync(NEW_IMEI);
        Assert.NotNull(actual);
        Assert.Equal(NEW_IMEI, actual.Imei);
        Assert.Matches("[0-9]{4}-[0-9]{2}-[0-9]{2} [0-9]{2}:[0-9]{2}:[0-9]{2}", lastUpdate);
    }
}
